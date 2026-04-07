const tomSelectInstances = {};
const tomSelectOptions = {};
const tomSelectDotNetCallbacks = {};
let tomSelectDebug = false;
let tomSelectObserver;

export function create(element, elementId, options, dotNetCallback) {
        // Destroy any existing instance first
        if (tomSelectInstances[elementId]) {
            destroy(elementId);
        }

        let tomSelect;
        let debug = false;

        if (options) {
            const opt = JSON.parse(options);
            debug = opt.debug || false;
            tomSelectDotNetCallbacks[elementId] = dotNetCallback;
            if (opt.useDotNetLoad === true) {
                opt.load = (query, callback) => {
                    try {
                        tomSelectDotNetCallbacks[elementId]
                            .invokeMethodAsync("LoadOptions", query)
                            .then(items => callback(items || []))
                            .catch(() => callback());
                    } catch (e) {
                        if (debug) console.warn("LoadOptions failed", e);
                        callback();
                    }
                };
            }
            if (typeof opt.shouldLoadMinQueryLength === 'number' && !isNaN(opt.shouldLoadMinQueryLength)) {
                const minLen = opt.shouldLoadMinQueryLength;
                opt.shouldLoad = (query) => (query || '').length >= minLen;
            }
            const getTemplateHtml = (el) => {
                if (!el) return null;
                // With hidden <div>, innerHTML is reliable
                return el.innerHTML;
            };
            const sanitizeTemplate = (html) => {
                if (!html) return html;
                // Remove Blazor comment markers and any HTML comments
                return html.replace(/<!--[^]*?-->/g, '').trim();
            };

            const applyTemplate = (tpl, data) => {
                if (!tpl) return null;
                let html = sanitizeTemplate(tpl);
                html = html.replace(/{{\s*([\w$.\-]+)\s*}}/g, (m, path) => {
                    const val = getByPath(data, path);
                    return (val === undefined || val === null) ? '' : String(val);
                });
                return html;
            };
            opt.onInitialize = async () => {
                try {
                    await dotNetCallback.invokeMethodAsync("OnInitializedJs");
                } catch (error) {
                    if (debug) {
                        console.warn(`Error calling OnInitializedJs for element ${elementId}:`, error);
                    }
                }
            };
            opt.render = opt.render || {};
            const defaultOptionRender = (item, escape) => {
                if (item.htmlOption) return item.htmlOption;
                const optionTemplateEl = document.getElementById(`${elementId}-option-template`);
                if (optionTemplateEl) {
                    const tpl = sanitizeTemplate(getTemplateHtml(optionTemplateEl));
                    const dataRoot = Object.assign({}, item || {}, (item && item.item) || {});
                    const html = tpl == null ? null : applyTemplate(tpl, dataRoot);

                    if (debug) {
                        console.log(`[TomSelectInterop] option template used for ${elementId}`, html);
                    }

                    if (html !== null)
                        return html;
                }

                if (debug) {
                    console.log(`[TomSelectInterop] option template not found or empty for ${elementId}, using label`);
                }
                const label = (item && (item[opt.labelField || 'text'] || item.text || ''));
                return `<div>${escape(label)}</div>`;
            };
            const defaultItemRender = (item, escape) => {
                if (item.htmlItem) return item.htmlItem;
                const itemTemplateEl = document.getElementById(`${elementId}-item-template`);
                if (itemTemplateEl) {
                    const tpl = sanitizeTemplate(getTemplateHtml(itemTemplateEl));
                    const dataRoot = Object.assign({}, item || {}, (item && item.item) || {});
                    const html = tpl == null ? null : applyTemplate(tpl, dataRoot);

                    if (debug) {
                        console.log(`[TomSelectInterop] item template used for ${elementId}`, html);
                    }

                    if (html !== null)
                        return html;
                }

                if (debug) {
                    console.log(`[TomSelectInterop] item template not found or empty for ${elementId}, using label`);
                }
                const label = (item && (item[opt.labelField || 'text'] || item.text || ''));
                return `<div>${escape(label)}</div>`;
            };
            // Always use our renderer; it will fallback to label if no template/HTML is present
            opt.render.option = defaultOptionRender;
            opt.render.item = defaultItemRender;
            tomSelect = new TomSelect(element, opt);
            tomSelectOptions[elementId] = opt;
        } else {
            tomSelect = new TomSelect(element, {
                onInitialize: async () => {
                    try {
                        await dotNetCallback.invokeMethodAsync("OnInitializedJs");
                    } catch (error) {
                        if (debug) {
                            console.warn(`Error calling OnInitializedJs for element ${elementId}:`, error);
                        }
                    }
                }
            });
        }

        tomSelectInstances[elementId] = tomSelect;
        tomSelectDebug = debug;
    };
export function addOption(elementId, data, userCreated) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.addOption(data, userCreated);
    };
export function addItem(elementId, value, silent) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.addItem(value, silent);
    };
export function addItems(elementId, data, silent) {
        const tomSelect = tomSelectInstances[elementId];
        data.forEach(item => tomSelect.addItem(item, silent));
    };
export function clearAndAddOptions(elementId, data, userCreated) {
        clearOptions(elementId);
        addOptions(elementId, data, userCreated);
    };
export function clearAndAddItems(elementId, data, silent) {
        clearItems(elementId, silent);
        addItems(elementId, data, silent);
    };
export function setOptions(elementId, options) {
        const tomSelect = tomSelectInstances[elementId];
        const opt = JSON.parse(options);
        tomSelectOptions[elementId] = opt;
        tomSelect.setOptions(opt);
    };
export function addOptions(elementId, data, userCreated) {
        const tomSelect = tomSelectInstances[elementId];
        data.forEach(option => tomSelect.addOption(option, userCreated));
    };
export function updateOption(elementId, value, data) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.updateOption(value, data);
    };
export function removeOption(elementId, value) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.removeOption(value);
    };
export function refreshOptions(elementId, triggerDropdown) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.refreshOptions(triggerDropdown);
    };
export function clearOptions(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.clearOptions();
    };
export function clearItems(elementId, silent) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.clear(silent);
    };
export function removeItem(elementId, valueOrHTMLElement, silent) {
        const tomSelect = tomSelectInstances[elementId];
        if (typeof valueOrHTMLElement === 'string' || valueOrHTMLElement instanceof String) {
            tomSelect.removeItem(valueOrHTMLElement, silent);
        } else {
            const itemValue = valueOrHTMLElement.getAttribute('data-value');
            tomSelect.removeItem(itemValue, silent);
        }
    };
export function refreshItems(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.refreshItems();
    };
export function addOptionGroup(elementId, id, data) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.addOptionGroup(id, data);
    };
export function removeOptionGroup(elementId, id) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.removeOptionGroup(id);
    };
export function clearOptionGroups(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.clearOptionGroups();
    };
export function lock(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.lock();
    };
export function unlock(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.unlock();
    };
export function enable(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.enable();
    };
export function disable(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.disable();
    };
export function setValue(elementId, value, silent) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.setValue(value, silent);
    };
export function getValue(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        return tomSelect.getValue();
    };
export function setCaret(elementId, index) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.setCaret(index);
    };
export function isFull(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        return tomSelect.isFull();
    };
export function clearCache(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.clearCache();
    };
export function setTextboxValue(elementId, str) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.setTextboxValue(str);
    };
export function sync(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.sync();
    };
export function destroy(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        if (tomSelect) {
            try {
                // Remove all event listeners before destroying
                tomSelect.off();
                tomSelect.destroy();
            } catch (error) {
                if (tomSelectDebug) {
                    console.warn(`Error destroying TomSelect for element ${elementId}:`, error);
                }
            } finally {
                delete tomSelectInstances[elementId];
                delete tomSelectOptions[elementId];
                delete tomSelectDotNetCallbacks[elementId];
            }
        }
    };
export function trigger(elementId, event) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.trigger(event);
    };
export function open(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.open();
    };
export function close(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.close();
    };
export function positionDropdown(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.positionDropdown();
    };
export function focus(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.focus();
    };
export function blur(elementId) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.blur();
    };
export function addEventListener(elementId, eventName, dotNetCallback) {
        const tomSelect = tomSelectInstances[elementId];
        tomSelect.on(eventName, async (...args) => {
            try {
                if (eventName === "item_select") {
                    return await dotNetCallback.invokeMethodAsync("Invoke", args[0].textContent);
                } else {
                    var json = getJsonFromArguments(...args);
                    return await dotNetCallback.invokeMethodAsync("Invoke", json);
                }
            } catch (error) {
                // Handle case where DotNetObjectReference is disposed
                if (error.message && error.message.includes("There is no tracked object")) {
                    if (tomSelectDebug) {
                        console.warn(`DotNetObjectReference disposed for element ${elementId}, removing event listener`);
                    }
                    tomSelect.off(eventName);
                    return;
                }
                throw error;
            }
        });
    };
export function getJsonFromArguments(...args) {
        const processedArgs = args.map(arg => {
            if (typeof arg === 'object' && arg !== null) {
                return objectToStringifyable(arg);
            } else {
                return arg;
            }
        });

        const json = JSON.stringify(processedArgs);
        return json;
    };
export function objectToStringifyable(obj) {
        let objectJSON = {};
        const props = Object.getOwnPropertyNames(obj);

        props.forEach(prop => {
            const descriptor = Object.getOwnPropertyDescriptor(obj, prop);
            if (descriptor && typeof descriptor.get === 'function') {
                objectJSON[prop] = descriptor.get.call(obj);
            } else {
                const propValue = obj[prop];
                if (typeof propValue === 'object' && propValue !== null) {
                    objectJSON[prop] = objectToStringifyable(propValue);
                } else {
                    objectJSON[prop] = propValue;
                }
            }
        });

        return objectJSON;
    };
export function createObserver(elementId) {
        const target = document.getElementById(elementId);
        if (!target) {
            if (tomSelectDebug) {
                console.warn(`Element with id ${elementId} not found for observer`);
            }
            return;
        }

        tomSelectObserver = new MutationObserver((mutations) => {
            const targetRemoved = mutations.some(mutation => Array.from(mutation.removedNodes).indexOf(target) !== -1);

            if (targetRemoved) {
                try {
                    destroy(elementId);
                } catch (error) {
                    if (tomSelectDebug) {
                        console.warn(`Error in mutation observer for element ${elementId}:`, error);
                    }
                }

                if (tomSelectObserver) {
                    tomSelectObserver.disconnect();
                    tomSelectObserver = null;
                }
            }
        });

        if (target.parentNode) {
            tomSelectObserver.observe(target.parentNode, { childList: true });
        }
    };
export function getByPath(obj, path) {
        try {
            const parts = String(path).split('.');
            let cur = obj;
            for (let i = 0; i < parts.length; i++) {
                if (cur == null) return undefined;
                cur = cur[parts[i]];
            }
            return cur;
        } catch {
            return undefined;
        }
    };
