export class TomSelectInterop {
    constructor() {
        this.tomSelects = {};
        this.options = {};
    }

    create(element, elementId, options, dotNetCallback) {
        // Destroy any existing instance first
        if (this.tomSelects[elementId]) {
            this.destroy(elementId);
        }

        let tomSelect;

        if (options) {
            const opt = JSON.parse(options);
            opt.onInitialize = async () => {
                try {
                    await dotNetCallback.invokeMethodAsync("OnInitializedJs");
                } catch (error) {
                    console.warn(`Error calling OnInitializedJs for element ${elementId}:`, error);
                }
            };
            tomSelect = new TomSelect(element, opt);
            this.options[elementId] = opt;
        } else {
            tomSelect = new TomSelect(element, {
                onInitialize: async () => {
                    try {
                        await dotNetCallback.invokeMethodAsync("OnInitializedJs");
                    } catch (error) {
                        console.warn(`Error calling OnInitializedJs for element ${elementId}:`, error);
                    }
                }
            });
        }

        this.tomSelects[elementId] = tomSelect;
    }

    addOption(elementId, data, userCreated) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.addOption(data, userCreated);
    }

    addItem(elementId, value, silent) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.addItem(value, silent);
    }

    addItems(elementId, data, silent) {
        const tomSelect = this.tomSelects[elementId];
        data.forEach(item => tomSelect.addItem(item, silent));
    }

    clearAndAddOptions(elementId, data, userCreated) {
        this.clearOptions(elementId);
        this.addOptions(elementId, data, userCreated);
    }

    clearAndAddItems(elementId, data, silent) {
        this.clearItems(elementId, silent);
        this.addItems(elementId, data, silent);
    }

    setOptions(elementId, options) {
        const tomSelect = this.tomSelects[elementId];
        const opt = JSON.parse(options);
        this.options[elementId] = opt;
        tomSelect.setOptions(opt);
    }

    addOptions(elementId, data, userCreated) {
        const tomSelect = this.tomSelects[elementId];
        data.forEach(option => tomSelect.addOption(option, userCreated));
    }

    updateOption(elementId, value, data) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.updateOption(value, data);
    }

    removeOption(elementId, value) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.removeOption(value);
    }

    refreshOptions(elementId, triggerDropdown) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.refreshOptions(triggerDropdown);
    }

    clearOptions(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.clearOptions();
    }

    clearItems(elementId, silent) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.clear(silent);
    }

    removeItem(elementId, valueOrHTMLElement, silent) {
        const tomSelect = this.tomSelects[elementId];
        if (typeof valueOrHTMLElement === 'string' || valueOrHTMLElement instanceof String) {
            tomSelect.removeItem(valueOrHTMLElement, silent);
        } else {
            const itemValue = valueOrHTMLElement.getAttribute('data-value');
            tomSelect.removeItem(itemValue, silent);
        }
    }

    refreshItems(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.refreshItems();
    }

    addOptionGroup(elementId, id, data) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.addOptionGroup(id, data);
    }

    removeOptionGroup(elementId, id) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.removeOptionGroup(id);
    }

    clearOptionGroups(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.clearOptionGroups();
    }

    lock(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.lock();
    }

    unlock(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.unlock();
    }

    enable(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.enable();
    }

    disable(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.disable();
    }

    setValue(elementId, value, silent) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.setValue(value, silent);
    }

    getValue(elementId) {
        const tomSelect = this.tomSelects[elementId];
        return tomSelect.getValue();
    }

    setCaret(elementId, index) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.setCaret(index);
    }

    isFull(elementId) {
        const tomSelect = this.tomSelects[elementId];
        return tomSelect.isFull();
    }

    clearCache(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.clearCache();
    }

    setTextboxValue(elementId, str) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.setTextboxValue(str);
    }

    sync(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.sync();
    }

    destroy(elementId) {
        const tomSelect = this.tomSelects[elementId];
        if (tomSelect) {
            try {
                // Remove all event listeners before destroying
                tomSelect.off();
                tomSelect.destroy();
            } catch (error) {
                console.warn(`Error destroying TomSelect for element ${elementId}:`, error);
            } finally {
                delete this.tomSelects[elementId];
                delete this.options[elementId];
            }
        }
    }

    trigger(elementId, event) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.trigger(event);
    }

    open(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.open();
    }

    close(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.close();
    }

    positionDropdown(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.positionDropdown();
    }

    focus(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.focus();
    }

    blur(elementId) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.blur();
    }

    addEventListener(elementId, eventName, dotNetCallback) {
        const tomSelect = this.tomSelects[elementId];
        tomSelect.on(eventName, async (...args) => {
            try {
                if (eventName === "item_select") {
                    return await dotNetCallback.invokeMethodAsync("Invoke", args[0].textContent);
                } else {
                    var json = this.getJsonFromArguments(...args);
                    return await dotNetCallback.invokeMethodAsync("Invoke", json);
                }
            } catch (error) {
                // Handle case where DotNetObjectReference is disposed
                if (error.message && error.message.includes("There is no tracked object")) {
                    console.warn(`DotNetObjectReference disposed for element ${elementId}, removing event listener`);
                    tomSelect.off(eventName);
                    return;
                }
                throw error;
            }
        });
    }

    getJsonFromArguments(...args) {
        const processedArgs = args.map(arg => {
            if (typeof arg === 'object' && arg !== null) {
                return this.objectToStringifyable(arg);
            } else {
                return arg;
            }
        });

        const json = JSON.stringify(processedArgs);
        return json;
    }

    objectToStringifyable(obj) {
        let objectJSON = {};
        const props = Object.getOwnPropertyNames(obj);

        props.forEach(prop => {
            const descriptor = Object.getOwnPropertyDescriptor(obj, prop);
            if (descriptor && typeof descriptor.get === 'function') {
                objectJSON[prop] = descriptor.get.call(obj);
            } else {
                const propValue = obj[prop];
                if (typeof propValue === 'object' && propValue !== null) {
                    objectJSON[prop] = this.objectToStringifyable(propValue);
                } else {
                    objectJSON[prop] = propValue;
                }
            }
        });

        return objectJSON;
    }

    createObserver(elementId) {
        const target = document.getElementById(elementId);
        if (!target) {
            console.warn(`Element with id ${elementId} not found for observer`);
            return;
        }

        this.observer = new MutationObserver((mutations) => {
            const targetRemoved = mutations.some(mutation => Array.from(mutation.removedNodes).indexOf(target) !== -1);

            if (targetRemoved) {
                try {
                    this.destroy(elementId);
                } catch (error) {
                    console.warn(`Error in mutation observer for element ${elementId}:`, error);
                }

                if (this.observer) {
                    this.observer.disconnect();
                    delete this.observer;
                }
            }
        });

        if (target.parentNode) {
            this.observer.observe(target.parentNode, { childList: true });
        }
    }
}

window.TomSelectInterop = new TomSelectInterop();