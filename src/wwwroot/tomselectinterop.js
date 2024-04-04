window.tomSelectInterop = (function () {
    var tomSelects = {};
    var options = {};

    function create(element, elementId, options, dotNetCallback) {
        var tomSelect;

        if (options) {
            var opt = JSON.parse(options);

            opt.onInitialize = function () {
                return dotNetCallback.invokeMethodAsync("OnInitializedJs");
            };

            tomSelect = new TomSelect(element, opt);
            options[elementId] = opt;
        }
        else
            tomSelect = new TomSelect(element, {
                onInitialize: function () {
                    return dotNetCallback.invokeMethodAsync("OnInitializedJs");
                }
            });

        tomSelects[elementId] = tomSelect;
    }

    function addOption(elementId, data, userCreated) {
        var tomSelect = tomSelects[elementId];

        tomSelect.addOption(data, userCreated);
    }

    function addItem(elementId, value, silent) {
        var tomSelect = tomSelects[elementId];

        tomSelect.addItem(value, silent);
    }

    function addItems(elementId, data, silent) {
        var tomSelect = tomSelects[elementId];
        data.forEach(item => tomSelect.addItem(item, silent));
    }

    function clearAndAddOptions(elementId, data, userCreated) {
        clearOptions(elementId);
        addOptions(elementId, data, userCreated);
    }

    function clearAndAddItems(elementId, data, silent) {
        clearItems(elementId, silent);
        addItems(elementId, data, silent);
    }

    function setOptions(elementId, options) {
        var tomSelect = tomSelects[elementId];

        var opt = JSON.parse(options);

        options[elementId] = opt;
        tomSelect.setOptions(opt);
    }

    function addOptions(elementId, data, userCreated) {
        var tomSelect = tomSelects[elementId];
        data.forEach(option => tomSelect.addOption(option, userCreated));
    }

    function updateOption(elementId, value, data) {
        var tomSelect = tomSelects[elementId];
        tomSelect.updateOption(value, data);
    }

    function removeOption(elementId, value) {
        var tomSelect = tomSelects[elementId];
        tomSelect.removeOption(value);
    }

    function refreshOptions(elementId, triggerDropdown) {
        var tomSelect = tomSelects[elementId];
        tomSelect.refreshOptions(triggerDropdown);
    }

    function clearOptions(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.clearOptions();
    }

    function clearItems(elementId, silent) {
        var tomSelect = tomSelects[elementId];
        tomSelect.clear(silent);
    }

    function removeItem(elementId, valueOrHTMLElement, silent) {
        var tomSelect = tomSelects[elementId];
        if (typeof valueOrHTMLElement === 'string' || valueOrHTMLElement instanceof String) {
            tomSelect.removeItem(valueOrHTMLElement, silent);
        } else {
            // Assumes valueOrHTMLElement is a DOM element
            var itemValue = valueOrHTMLElement.getAttribute('data-value');
            tomSelect.removeItem(itemValue, silent);
        }
    }

    function refreshItems(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.refreshItems();
    }

    function addOptionGroup(elementId, id, data) {
        var tomSelect = tomSelects[elementId];
        tomSelect.addOptionGroup(id, data);
    }

    function removeOptionGroup(elementId, id) {
        var tomSelect = tomSelects[elementId];
        tomSelect.removeOptionGroup(id);
    }

    function clearOptionGroups(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.clearOptionGroups();
    }

    function lock(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.lock();
    }

    function unlock(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.unlock();
    }

    function enable(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.enable();
    }

    function disable(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.disable();
    }

    function setValue(elementId, value, silent) {
        var tomSelect = tomSelects[elementId];
        tomSelect.setValue(value, silent);
    }

    function getValue(elementId) {
        var tomSelect = tomSelects[elementId];
        return tomSelect.getValue();
    }

    function setCaret(elementId, index) {
        var tomSelect = tomSelects[elementId];
        tomSelect.setCaret(index);
    }

    function isFull(elementId) {
        var tomSelect = tomSelects[elementId];
        return tomSelect.isFull();
    }

    function clearCache(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.clearCache();
    }

    function setTextboxValue(elementId, str) {
        var tomSelect = tomSelects[elementId];
        tomSelect.setTextboxValue(str);
    }

    function sync(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.sync();
    }

    function destroy(elementId) {
        var tomSelect = tomSelects[elementId];
        if (tomSelect) {
            tomSelect.destroy();
            tomSelects[elementId] = null;
        }
    }

    function trigger(elementId, event) {
        var tomSelect = tomSelects[elementId];
        tomSelect.trigger(event);
    }

    function open(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.open();
    }

    function close(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.close();
    }

    function positionDropdown(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.positionDropdown();
    }

    function focus(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.focus();
    }

    function blur(elementId) {
        var tomSelect = tomSelects[elementId];
        tomSelect.blur();
    }

    function addEventListener(elementId, eventName, dotNetCallback) {
        var tomSelect = tomSelects[elementId];
        tomSelect.on(eventName, function (...args) {
            var json = getJsonFromArguments(...args);

            return dotNetCallback.invokeMethodAsync("Invoke", json);
        });
    }

    function getJsonFromArguments(...args) {
        const processedArgs = args.map(arg => {
            if (typeof arg === 'object' && arg !== null) {
                return objectToStringifyable(arg);
            } else {
                return arg;
            }
        });

        var json = JSON.stringify(processedArgs);
        return json;
    }

    function objectToStringifyable(obj) {
        let objectJSON = {};

        // Get all property names of the object
        let props = Object.getOwnPropertyNames(obj);

        // Iterate through each property
        props.forEach(prop => {
            // Get the property descriptor
            let descriptor = Object.getOwnPropertyDescriptor(obj, prop);

            // Check if the property has a getter
            if (descriptor && typeof descriptor.get === 'function') {
                // Call the getter in the context of the object
                objectJSON[prop] = descriptor.get.call(obj);
            } else {
                // Include the property value if there's no getter
                const propValue = obj[prop];

                if (typeof propValue === 'object' && propValue !== null) {
                    // Recursively handle nested objects
                    objectJSON[prop] = objectToStringifyable(propValue);
                } else {
                    objectJSON[prop] = propValue;
                }
            }
        });

        return objectJSON;
    }

    return {
        create: create,
        addOption: addOption,
        addItem: addItem,
        addItems: addItems,
        addOptions: addOptions,
        updateOption: updateOption,
        removeOption: removeOption,
        refreshOptions: refreshOptions,
        clearOptions: clearOptions,
        clearItems: clearItems,
        clearAndAddItems: clearAndAddItems,
        clearAndAddOptions: clearAndAddOptions,

        removeItem: removeItem,
        refreshItems: refreshItems,
        addOptionGroup: addOptionGroup,
        removeOptionGroup: removeOptionGroup,
        clearOptionGroups: clearOptionGroups,
        lock: lock,
        unlock: unlock,
        enable: enable,
        disable: disable,
        setValue: setValue,
        getValue: getValue,
        setCaret: setCaret,
        isFull: isFull,
        clearCache: clearCache,
        setTextboxValue: setTextboxValue,
        sync: sync,
/*        on: on,*/
/*        off: off,*/
        trigger: trigger,
        open: open,
        close: close,
        positionDropdown: positionDropdown,
        focus: focus,
        blur: blur,
/*        load: load,*/
        destroy: destroy,
        setOptions: setOptions,
        addEventListener: addEventListener,
    };
})();