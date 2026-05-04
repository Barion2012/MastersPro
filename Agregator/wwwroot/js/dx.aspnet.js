/*!
* DevExtreme (dx.aspnet.mvc.js)
* Version: 20.2.6
* Build date: Tue Mar 16 2021
*
* Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
! function(factory) {
    if ("function" === typeof define && define.amd) {
        define(function(require, exports, module) {
            module.exports = factory(require("jquery"), require("./core/templates/template_engine_registry").setTemplateEngine, require("./core/templates/template_base").renderedCallbacks, require("./core/guid"), require("./ui/validation_engine"), require("./core/utils/iterator"), require("./core/utils/dom").extractTemplateMarkup, require("./core/utils/string").encodeHtml, require("./core/utils/ajax"), require("./core/utils/console"))
        })
    } else {
        DevExpress.aspnet = factory(window.jQuery, DevExpress.setTemplateEngine, DevExpress.templateRendered, DevExpress.data.Guid, DevExpress.validationEngine, DevExpress.utils.iterator, DevExpress.utils.dom.extractTemplateMarkup, DevExpress.utils.string.encodeHtml, DevExpress.utils.ajax, DevExpress.utils.console)
    }
}(function($, setTemplateEngine, templateRendered, Guid, validationEngine, iteratorUtils, extractTemplateMarkup, encodeHtml, ajax, console) {
    var templateCompiler = createTemplateCompiler();
    var pendingCreateComponentRoutines = [];
    var enableAlternativeTemplateTags = true;
    var warnBug17028 = false;

    function createTemplateCompiler() {
        var OPEN_TAG = "<%",
            CLOSE_TAG = "%>",
            ENCODE_QUALIFIER = "-",
            INTERPOLATE_QUALIFIER = "=";
        var EXTENDED_OPEN_TAG = /[<[]%/g,
            EXTENDED_CLOSE_TAG = /%[>\]]/g;

        function acceptText(bag, text) {
            if (text) {
                bag.push("_.push(", JSON.stringify(text), ");")
            }
        }

        function acceptCode(bag, code) {
            var encode = code.charAt(0) === ENCODE_QUALIFIER,
                value = code.substr(1),
                interpolate = code.charAt(0) === INTERPOLATE_QUALIFIER;
            if (encode || interpolate) {
                bag.push("_.push(");
                var expression = value;
                if (encode) {
                    expression = "arguments[1]((" + value + " !== null && " + value + " !== undefined) ? " + value + ' : "")';
                    if (/^\s*$/.test(value)) {
                        expression = "arguments[1](" + value + ")"
                    }
                }
                bag.push(expression);
                bag.push(");")
            } else {
                bag.push(code + "\n")
            }
        }
        return function(text) {
            var bag = ["var _ = [];", "with(obj||{}) {"],
                chunks = text.split(enableAlternativeTemplateTags ? EXTENDED_OPEN_TAG : OPEN_TAG);
            if (warnBug17028 && chunks.length > 1) {
                if (text.indexOf(OPEN_TAG) > -1) {
                    console.logger.warn("Please use an alternative template syntax: https://community.devexpress.com/blogs/aspnet/archive/2020/01/29/asp-net-core-new-syntax-to-fix-razor-issue.aspx");
                    warnBug17028 = false
                }
            }
            acceptText(bag, chunks.shift());
            for (var i = 0; i < chunks.length; i++) {
                var tmp = chunks[i].split(enableAlternativeTemplateTags ? EXTENDED_CLOSE_TAG : CLOSE_TAG);
                if (2 !== tmp.length) {
                    throw "Template syntax error"
                }
                acceptCode(bag, tmp[0]);
                acceptText(bag, tmp[1])
            }
            bag.push("}", "return _.join('')");
            return new Function("obj", bag.join(""))
        }
    }

    function createTemplateEngine() {
        return {
            compile: function(element) {
                return templateCompiler(extractTemplateMarkup(element))
            },
            render: function(template, data) {
                var html = template(data, encodeHtml);
                var dxMvcExtensionsObj = window.MVCx;
                if (dxMvcExtensionsObj && !dxMvcExtensionsObj.isDXScriptInitializedOnLoad) {
                    html = html.replace(/(<script[^>]+)id="dxss_.+?"/g, "$1")
                }
                return html
            }
        }
    }

    function getValidationSummary(validationGroup) {
        var result;
        $(".dx-validationsummary").each(function(_, element) {
            var summary = $(element).data("dxValidationSummary");
            if (summary && summary.option("validationGroup") === validationGroup) {
                result = summary;
                return false
            }
        });
        return result
    }

    function createValidationSummaryItemsFromValidators(validators, editorNames) {
        var items = [];
        iteratorUtils.each(validators, function(_, validator) {
            var widget = validator.$element().data("dx-validation-target");
            if (widget && $.inArray(widget.option("name"), editorNames) > -1) {
                items.push({
                    text: widget.option("validationError.message"),
                    validator: validator
                })
            }
        });
        return items
    }

    function createComponent(name, options, id, validatorOptions) {
        var selector = "#" + String(id).replace(/[^\w-]/g, "\\$&");
        pendingCreateComponentRoutines.push(function() {
            var $element = $(selector);
            if ($element.length) {
                var $component = $(selector)[name](options);
                if ($.isPlainObject(validatorOptions)) {
                    $component.dxValidator(validatorOptions)
                }
                return true
            }
            return false
        })
    }
    templateRendered.add(function() {
        var snapshot = pendingCreateComponentRoutines.slice();
        var leftover = [];
        pendingCreateComponentRoutines = [];
        snapshot.forEach(function(func) {
            if (!func()) {
                leftover.push(func)
            }
        });
        pendingCreateComponentRoutines = pendingCreateComponentRoutines.concat(leftover)
    });
    return {
        createComponent: createComponent,
        renderComponent: function(name, options, id, validatorOptions) {
            id = id || "dx-" + new Guid;
            createComponent(name, options, id, validatorOptions);
            return '<div id="' + id + '"></div>'
        },
        getEditorValue: function(inputName) {
            var $widget = $("input[name='" + inputName + "']").closest(".dx-widget");
            if ($widget.length) {
                var dxComponents = $widget.data("dxComponents"),
                    widget = $widget.data(dxComponents[0]);
                if (widget) {
                    return widget.option("value")
                }
            }
        },
        setTemplateEngine: function() {
            if (setTemplateEngine) {
                setTemplateEngine(createTemplateEngine())
            }
        },
        enableAlternativeTemplateTags: function(value) {
            enableAlternativeTemplateTags = value
        },
        warnBug17028: function() {
            warnBug17028 = true
        },
        createValidationSummaryItems: function(validationGroup, editorNames) {
            var groupConfig, items, summary = getValidationSummary(validationGroup);
            if (summary) {
                groupConfig = validationEngine.getGroupConfig(validationGroup);
                if (groupConfig) {
                    items = createValidationSummaryItemsFromValidators(groupConfig.validators, editorNames);
                    items.length && summary.option("items", items)
                }
            }
        },
        sendValidationRequest: function(propertyName, propertyValue, url, method) {
            var d = $.Deferred();
            var data = {};
            data[propertyName] = propertyValue;
            ajax.sendRequest({
                url: url,
                dataType: "json",
                method: method || "GET",
                data: data
            }).then(function(response) {
                if ("string" === typeof response) {
                    d.resolve({
                        isValid: false,
                        message: response
                    })
                } else {
                    d.resolve(response)
                }
            }, function(xhr) {
                d.reject({
                    isValid: false,
                    message: xhr.responseText
                })
            });
            return d.promise()
        }
    }
});

// Version: 2.7.1
// https://github.com/DevExpress/DevExtreme.AspNet.Data
// Copyright (c) Developer Express Inc.

/* global DevExpress:false, jQuery:false */

(function(factory) {
    "use strict";

    if(typeof define === "function" && define.amd) {
        define(function(require, exports, module) {
            module.exports = factory(
                require("devextreme/core/utils/ajax"),
                require("jquery").Deferred,
                require("jquery").extend,
                require("devextreme/data/custom_store"),
                require("devextreme/data/utils")
            );
        });
    } else if (typeof module === "object" && module.exports) {
        module.exports = factory(
            require("devextreme/core/utils/ajax"),
            require("jquery").Deferred,
            require("jquery").extend,
            require("devextreme/data/custom_store"),
            require("devextreme/data/utils")
        );
    } else {
        DevExpress.data.AspNet = factory(
            DevExpress.utils.ajax || { sendRequest: jQuery.ajax },
            jQuery.Deferred,
            jQuery.extend,
            DevExpress.data.CustomStore,
            DevExpress.data.utils
        );
    }

})(function(ajaxUtility, Deferred, extend, CustomStore, dataUtils) {
    "use strict";

    var CUSTOM_STORE_OPTIONS = [
        "onLoading", "onLoaded",
        "onInserting", "onInserted",
        "onUpdating", "onUpdated",
        "onRemoving", "onRemoved",
        "onModifying", "onModified",
        "onPush",
        "loadMode", "cacheRawData",
        "errorHandler"
    ];

    function createStoreConfig(options) {
        var keyExpr = options.key,
            loadUrl = options.loadUrl,
            loadMethod = options.loadMethod || "GET",
            loadParams = options.loadParams,
            isRawLoadMode = options.loadMode === "raw",
            updateUrl = options.updateUrl,
            insertUrl = options.insertUrl,
            deleteUrl = options.deleteUrl,
            onBeforeSend = options.onBeforeSend,
            onAjaxError = options.onAjaxError;

        function send(operation, requiresKey, ajaxSettings, customSuccessHandler) {
            var d = Deferred(),
                thenable,
                beforeSendResult;

            function sendCore() {
                ajaxUtility.sendRequest(ajaxSettings).then(
                    function(res, textStatus, xhr) {
                        if(customSuccessHandler)
                            customSuccessHandler(d, res, xhr);
                        else
                            d.resolve();
                    },
                    function(xhr, textStatus) {
                        var error = getErrorMessageFromXhr(xhr);

                        if(onAjaxError) {
                            var e = { xhr: xhr, error: error };
                            onAjaxError(e);
                            error = e.error;
                        }

                        if(error)
                            d.reject(error);
                        else
                            d.reject(xhr, textStatus);
                    }
                );
            }

            if(requiresKey && !keyExpr) {
                d.reject(new Error("Primary key is not specified (operation: '" + operation + "', url: '" + ajaxSettings.url + "')"));
            } else {
                if(operation === "load") {
                    ajaxSettings.cache = false;
                    ajaxSettings.dataType = "json";
                } else {
                    ajaxSettings.dataType = "text";
                }

                if(onBeforeSend) {
                    beforeSendResult = onBeforeSend(operation, ajaxSettings);
                    if(beforeSendResult && typeof beforeSendResult.then === "function")
                        thenable = beforeSendResult;
                }

                if(thenable)
                    thenable.then(sendCore, function(error) { d.reject(error); });
                else
                    sendCore();
            }

            return d.promise();
        }

        function filterByKey(keyValue) {
            if(!Array.isArray(keyExpr))
                return [keyExpr, keyValue];

            return keyExpr.map(function(i) {
                return [i, keyValue[i]];
            });
        }

        function loadOptionsToActionParams(options, isCountQuery) {
            var result = {};

            if(isCountQuery)
                result.isCountQuery = isCountQuery;

            if(options) {

                ["skip", "take", "requireTotalCount", "requireGroupCount"].forEach(function(i) {
                    if(options[i] !== undefined)
                        result[i] = options[i];
                });

                var normalizeSorting = dataUtils.normalizeSortingInfo,
                    group = options.group,
                    filter = options.filter,
                    select = options.select;

                if(options.sort)
                    result.sort = JSON.stringify(normalizeSorting(options.sort));

                if(group) {
                    if(!isAdvancedGrouping(group))
                        group = normalizeSorting(group);
                    result.group = JSON.stringify(group);
                }

                if(Array.isArray(filter)) {
                    filter = extend(true, [], filter);
                    stringifyDatesInFilter(filter);
                    result.filter = JSON.stringify(filter);
                }

                if(options.totalSummary)
                    result.totalSummary = JSON.stringify(options.totalSummary);

                if(options.groupSummary)
                    result.groupSummary = JSON.stringify(options.groupSummary);

                if(select) {
                    if(!Array.isArray(select))
                        select = [ select ];
                    result.select = JSON.stringify(select);
                }
            }

            extend(result, loadParams);

            return result;
        }

        function handleInsertUpdateSuccess(d, res, xhr) {
            var mime = xhr.getResponseHeader("Content-Type"),
                isJSON = mime && mime.indexOf("application/json") > -1;
            d.resolve(isJSON ? JSON.parse(res) : res);
        }

        var result = {
            key: keyExpr,
            useDefaultSearch: true,

            load: function(loadOptions) {
                return send(
                    "load",
                    false,
                    {
                        url: loadUrl,
                        method: loadMethod,
                        data: loadOptionsToActionParams(loadOptions)
                    },
                    function(d, res) {
                        processLoadResponse(d, res, function(res) {
                            return [ res.data, createLoadExtra(res) ];
                        });
                    }
                );
            },

            totalCount: !isRawLoadMode && function(loadOptions) {
                return send(
                    "load",
                    false,
                    {
                        url: loadUrl,
                        method: loadMethod,
                        data: loadOptionsToActionParams(loadOptions, true)
                    },
                    function(d, res) {
                        processLoadResponse(d, res, function(res) {
                            return [ res.totalCount ];
                        });
                    }
                );
            },

            byKey: !isRawLoadMode && function(key) {
                return send(
                    "load",
                    true,
                    {
                        url: loadUrl,
                        method: loadMethod,
                        data: loadOptionsToActionParams({ filter: filterByKey(key) })
                    },
                    function(d, res) {
                        processLoadResponse(d, res, function(res) {
                            return [ res.data[0] ];
                        });
                    }
                );
            },

            update: updateUrl && function(key, values) {
                return send(
                    "update",
                    true,
                    {
                        url: updateUrl,
                        method: options.updateMethod || "PUT",
                        data: {
                            key: serializeKey(key),
                            values: JSON.stringify(values)
                        }
                    },
                    handleInsertUpdateSuccess
                );
            },

            insert: insertUrl && function(values) {
                return send(
                    "insert",
                    true,
                    {
                        url: insertUrl,
                        method: options.insertMethod || "POST",
                        data: { values: JSON.stringify(values) }
                    },
                    handleInsertUpdateSuccess
                );
            },

            remove: deleteUrl && function(key) {
                return send("delete", true, {
                    url: deleteUrl,
                    method: options.deleteMethod || "DELETE",
                    data: { key: serializeKey(key) }
                });
            }

        };

        CUSTOM_STORE_OPTIONS.forEach(function(name) {
            var value = options[name];
            if(value !== undefined)
                result[name] = value;
        });

        return result;
    }

    function processLoadResponse(d, res, getResolveArgs) {
        res = expandLoadResponse(res);

        if(!res || typeof res !== "object")
            d.reject(new Error("Unexpected response received"));
        else
            d.resolve.apply(d, getResolveArgs(res));
    }

    function expandLoadResponse(value) {
        if(Array.isArray(value))
            return { data: value };

        if(typeof value === "number")
            return { totalCount: value };

        return value;
    }

    function createLoadExtra(res) {
        return {
            totalCount: "totalCount" in res ? res.totalCount : -1,
            groupCount: "groupCount" in res ? res.groupCount : -1,
            summary: res.summary || null
        };
    }

    function serializeKey(key) {
        if(typeof key === "object")
            return JSON.stringify(key);

        return key;
    }

    function serializeDate(date) {

        function zpad(text, len) {
            text = String(text);
            while(text.length < len)
                text = "0" + text;
            return text;
        }

        var builder = [1 + date.getMonth(), "/", date.getDate(), "/", date.getFullYear()],
            h = date.getHours(),
            m = date.getMinutes(),
            s = date.getSeconds(),
            f = date.getMilliseconds();

        if(h + m + s + f > 0)
            builder.push(" ", zpad(h, 2), ":", zpad(m, 2), ":", zpad(s, 2), ".", zpad(f, 3));

        return builder.join("");
    }

    function stringifyDatesInFilter(crit) {
        crit.forEach(function(v, k) {
            if(Array.isArray(v)) {
                stringifyDatesInFilter(v);
            } else if(Object.prototype.toString.call(v) === "[object Date]") {
                crit[k] = serializeDate(v);
            }
        });
    }

    function isAdvancedGrouping(expr) {
        if(!Array.isArray(expr))
            return false;

        for(var i = 0; i < expr.length; i++) {
            if("groupInterval" in expr[i] || "isExpanded" in expr[i])
                return true;
        }

        return false;
    }

    function getErrorMessageFromXhr(xhr) {
        var mime = xhr.getResponseHeader("Content-Type"),
            responseText = xhr.responseText;

        if(!mime)
            return null;

        if(mime.indexOf("text/plain") === 0)
            return responseText;

        if(mime.indexOf("application/json") === 0) {
            var jsonObj = safeParseJSON(responseText);

            if(typeof jsonObj === "string")
                return jsonObj;

            if(typeof jsonObj === "object") {
                for(var key in jsonObj) {
                    if(typeof jsonObj[key] === "string")
                        return jsonObj[key];
                }
            }

            return responseText;
        }

        return null;
    }

    function safeParseJSON(json) {
        try {
            return JSON.parse(json);
        } catch(x) {
            return null;
        }
    }

    return {
        createStore: function(options) {
            return new CustomStore(createStoreConfig(options));
        }
    };
});

/*!
* DevExtreme (dx.messages.ru.js)
* Version: 20.2.6
* Build date: Tue Mar 16 2021
*
* Copyright (c) 2012 - 2021 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
"use strict";

! function(root, factory) {
    if ("function" === typeof define && define.amd) {
        define(function(require) {
            factory(require("devextreme/localization"))
        })
    } else {
        if ("object" === typeof module && module.exports) {
            factory(require("devextreme/localization"))
        } else {
            factory(DevExpress.localization)
        }
    }
}(this, function(localization) {
    localization.loadMessages({
        ru: {
            Yes: "\u0414\u0430",
            No: "\u041d\u0435\u0442",
            Cancel: "\u041e\u0442\u043c\u0435\u043d\u0430",
            Clear: "\u041e\u0447\u0438\u0441\u0442\u0438\u0442\u044c",
            Done: "\u0413\u043e\u0442\u043e\u0432\u043e",
            Loading: "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430...",
            Select: "\u0412\u044b\u0431\u0440\u0430\u0442\u044c...",
            Search: "\u041f\u043e\u0438\u0441\u043a",
            Back: "\u041d\u0430\u0437\u0430\u0434",
            OK: "OK",
            "dxCollectionWidget-noDataText": "\u041d\u0435\u0442 \u0434\u0430\u043d\u043d\u044b\u0445 \u0434\u043b\u044f \u043e\u0442\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u044f",
            "dxDropDownEditor-selectLabel": "\u0412\u044b\u0431\u0440\u0430\u0442\u044c",
            "validation-required": "\u041f\u043e\u043b\u0435 \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c\u043e \u0437\u0430\u043f\u043e\u043b\u043d\u0438\u0442\u044c",
            "validation-required-formatted": "\u041d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c\u043e \u0437\u0430\u043f\u043e\u043b\u043d\u0438\u0442\u044c: {0}",
            "validation-numeric": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0447\u0438\u0441\u043b\u043e\u043c",
            "validation-numeric-formatted": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043f\u043e\u043b\u044f {0} \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0447\u0438\u0441\u043b\u043e\u043c",
            "validation-range": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043f\u043e\u043b\u044f \u043d\u0435 \u0432\u0445\u043e\u0434\u0438\u0442 \u0432 \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d",
            "validation-range-formatted": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043f\u043e\u043b\u044f {0} \u043d\u0435 \u0432\u0445\u043e\u0434\u0438\u0442 \u0432 \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d",
            "validation-stringLength": "\u041d\u0435\u0432\u0435\u0440\u043d\u0430\u044f \u0434\u043b\u0438\u043d\u0430 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f \u0432 \u043f\u043e\u043b\u0435",
            "validation-stringLength-formatted": "\u041d\u0435\u0432\u0435\u0440\u043d\u0430\u044f \u0434\u043b\u0438\u043d\u0430 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f \u0432 \u043f\u043e\u043b\u0435 {0}",
            "validation-custom": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435",
            "validation-custom-formatted": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435: {0}",
            "validation-async": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435",
            "validation-async-formatted": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435: {0}",
            "validation-compare": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u044f \u043f\u043e\u043b\u0435\u0439 \u043d\u0435 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0443\u044e\u0442 \u0434\u0440\u0443\u0433 \u0434\u0440\u0443\u0433\u0443.",
            "validation-compare-formatted": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043f\u043e\u043b\u044f {0} \u043d\u0435 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0443\u0435\u0442",
            "validation-pattern": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043d\u0435 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0448\u0430\u0431\u043b\u043e\u043d\u0443",
            "validation-pattern-formatted": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043f\u043e\u043b\u044f {0} \u043d\u0435 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0448\u0430\u0431\u043b\u043e\u043d\u0443",
            "validation-email": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 email",
            "validation-email-formatted": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438e {0}",
            "validation-mask": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435",
            "dxLookup-searchPlaceholder": "\u041c\u0438\u043d\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u043a\u043e\u043b\u0438\u0447\u0435\u0441\u0442\u0432\u043e \u0441\u0438\u043c\u0432\u043e\u043b\u043e\u0432: {0}",
            "dxList-pullingDownText": "\u041f\u043e\u0442\u044f\u043d\u0438\u0442\u0435, \u0447\u0442\u043e\u0431\u044b \u043e\u0431\u043d\u043e\u0432\u0438\u0442\u044c...",
            "dxList-pulledDownText": "\u041e\u0442\u043f\u0443\u0441\u0442\u0438\u0442\u0435, \u0447\u0442\u043e\u0431\u044b \u043e\u0431\u043d\u043e\u0432\u0438\u0442\u044c...",
            "dxList-refreshingText": "\u041e\u0431\u043d\u043e\u0432\u043b\u0435\u043d\u0438\u0435...",
            "dxList-pageLoadingText": "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430...",
            "dxList-nextButtonText": "\u0414\u0430\u043b\u0435\u0435",
            "dxList-selectAll": "\u0412\u044b\u0431\u0440\u0430\u0442\u044c \u0432\u0441\u0435",
            "dxListEditDecorator-delete": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            "dxListEditDecorator-more": "\u0415\u0449\u0435",
            "dxScrollView-pullingDownText": "\u041f\u043e\u0442\u044f\u043d\u0438\u0442\u0435, \u0447\u0442\u043e\u0431\u044b \u043e\u0431\u043d\u043e\u0432\u0438\u0442\u044c...",
            "dxScrollView-pulledDownText": "\u041e\u0442\u043f\u0443\u0441\u0442\u0438\u0442\u0435, \u0447\u0442\u043e\u0431\u044b \u043e\u0431\u043d\u043e\u0432\u0438\u0442\u044c...",
            "dxScrollView-refreshingText": "\u041e\u0431\u043d\u043e\u0432\u043b\u0435\u043d\u0438\u0435...",
            "dxScrollView-reachBottomText": "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430...",
            "dxDateBox-simulatedDataPickerTitleTime": "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0432\u0440\u0435\u043c\u044f",
            "dxDateBox-simulatedDataPickerTitleDate": "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0434\u0430\u0442\u0443",
            "dxDateBox-simulatedDataPickerTitleDateTime": "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0434\u0430\u0442\u0443 \u0438 \u0432\u0440\u0435\u043c\u044f",
            "dxDateBox-validation-datetime": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0434\u0430\u0442\u043e\u0439/\u0432\u0440\u0435\u043c\u0435\u043d\u0435\u043c",
            "dxFileUploader-selectFile": "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0444\u0430\u0439\u043b",
            "dxFileUploader-dropFile": "\u0438\u043b\u0438 \u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u0444\u0430\u0439\u043b \u0441\u044e\u0434\u0430",
            "dxFileUploader-bytes": "\u0431\u0430\u0439\u0442",
            "dxFileUploader-kb": "\u043a\u0411",
            "dxFileUploader-Mb": "\u041c\u0411",
            "dxFileUploader-Gb": "\u0413\u0411",
            "dxFileUploader-upload": "\u0417\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c",
            "dxFileUploader-uploaded": "\u0417\u0430\u0433\u0440\u0443\u0436\u0435\u043d\u043e",
            "dxFileUploader-readyToUpload": "\u0413\u043e\u0442\u043e\u0432\u043e \u043a \u0437\u0430\u0433\u0440\u0443\u0437\u043a\u0435",
            "dxFileUploader-uploadAbortedMessage": "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430 \u043e\u0442\u043c\u0435\u043d\u0435\u043d\u0430",
            "dxFileUploader-uploadFailedMessage": "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430 \u043d\u0435 \u0443\u0434\u0430\u043b\u0430\u0441\u044c",
            "dxFileUploader-invalidFileExtension": "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0435 \u0444\u0430\u0439\u043b\u0430",
            "dxFileUploader-invalidMaxFileSize": "\u0424\u0430\u0439\u043b \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u0431\u043e\u043b\u044c\u0448\u043e\u0439",
            "dxFileUploader-invalidMinFileSize": "\u0424\u0430\u0439\u043b \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u0430\u043b\u0435\u043d\u044c\u043a\u0438\u0439",
            "dxRangeSlider-ariaFrom": "\u041e\u0442",
            "dxRangeSlider-ariaTill": "\u0414\u043e",
            "dxSwitch-switchedOnText": "\u0412\u041a\u041b",
            "dxSwitch-switchedOffText": "\u0412\u042b\u041a\u041b",
            "dxForm-optionalMark": "\u043d\u0435\u043e\u0431\u044f\u0437\u0430\u0442\u0435\u043b\u044c\u043d\u044b\u0439",
            "dxForm-requiredMessage": " \u041f\u043e\u043b\u0435 {0} \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0437\u0430\u043f\u043e\u043b\u043d\u0435\u043d\u043e",
            "dxNumberBox-invalidValueMessage": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0447\u0438\u0441\u043b\u043e\u043c",
            "dxNumberBox-noDataText": "\u041d\u0435\u0442 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxDataGrid-columnChooserTitle": "\u0412\u044b\u0431\u043e\u0440 \u0441\u0442\u043e\u043b\u0431\u0446\u043e\u0432",
            "dxDataGrid-columnChooserEmptyText": "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u0441\u0442\u043e\u043b\u0431\u0435\u0446 \u0441\u044e\u0434\u0430, \u0447\u0442\u043e\u0431\u044b \u0441\u043a\u0440\u044b\u0442\u044c \u0435\u0433\u043e",
            "dxDataGrid-groupContinuesMessage": "\u041f\u0440\u043e\u0434\u043e\u043b\u0436\u0435\u043d\u0438\u0435 \u043d\u0430 \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0435\u0439 \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u0435",
            "dxDataGrid-groupContinuedMessage": "\u041f\u0440\u043e\u0434\u043e\u043b\u0436\u0435\u043d\u0438\u0435 \u0441 \u043f\u0440\u0435\u0434\u044b\u0434\u0443\u0449\u0435\u0439 \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u044b",
            "dxDataGrid-groupHeaderText": "\u0421\u0433\u0440\u0443\u043f\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0434\u0430\u043d\u043d\u044b\u0435 \u043f\u043e \u044d\u0442\u043e\u043c\u0443 \u0441\u0442\u043e\u043b\u0431\u0446\u0443",
            "dxDataGrid-ungroupHeaderText": "\u0420\u0430\u0437\u0433\u0440\u0443\u043f\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0434\u0430\u043d\u043d\u044b\u0435 \u043f\u043e \u044d\u0442\u043e\u043c\u0443 \u0441\u0442\u043e\u043b\u0431\u0446\u0443",
            "dxDataGrid-ungroupAllText": "\u0421\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0433\u0440\u0443\u043f\u043f\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435",
            "dxDataGrid-editingEditRow": "\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c",
            "dxDataGrid-editingSaveRowChanges": "\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c",
            "dxDataGrid-editingCancelRowChanges": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c",
            "dxDataGrid-editingDeleteRow": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            "dxDataGrid-editingUndeleteRow": "\u0412\u043e\u0441\u0441\u0442\u0430\u043d\u043e\u0432\u0438\u0442\u044c",
            "dxDataGrid-editingConfirmDeleteMessage": "\u0412\u044b \u0443\u0432\u0435\u0440\u0435\u043d\u044b, \u0447\u0442\u043e \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c \u044d\u0442\u0443 \u0437\u0430\u043f\u0438\u0441\u044c?",
            "dxDataGrid-validationCancelChanges": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f",
            "dxDataGrid-groupPanelEmptyText": "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u0441\u0442\u043e\u043b\u0431\u0435\u0446 \u0441\u044e\u0434\u0430, \u0447\u0442\u043e\u0431\u044b \u0441\u0433\u0440\u0443\u043f\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u043f\u043e \u043d\u0435\u043c\u0443",
            "dxDataGrid-noDataText": "\u041d\u0435\u0442 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxDataGrid-searchPanelPlaceholder": "\u0418\u0441\u043a\u0430\u0442\u044c...",
            "dxDataGrid-filterRowShowAllText": "(\u0412\u0441\u0435)",
            "dxDataGrid-filterRowResetOperationText": "\u0421\u0431\u0440\u043e\u0441\u0438\u0442\u044c",
            "dxDataGrid-filterRowOperationEquals": "\u0420\u0430\u0432\u043d\u043e",
            "dxDataGrid-filterRowOperationNotEquals": "\u041d\u0435 \u0440\u0430\u0432\u043d\u043e",
            "dxDataGrid-filterRowOperationLess": "\u041c\u0435\u043d\u044c\u0448\u0435",
            "dxDataGrid-filterRowOperationLessOrEquals": "\u041c\u0435\u043d\u044c\u0448\u0435 \u0438\u043b\u0438 \u0440\u0430\u0432\u043d\u043e",
            "dxDataGrid-filterRowOperationGreater": "\u0411\u043e\u043b\u044c\u0448\u0435",
            "dxDataGrid-filterRowOperationGreaterOrEquals": "\u0411\u043e\u043b\u044c\u0448\u0435 \u0438\u043b\u0438 \u0440\u0430\u0432\u043d\u043e",
            "dxDataGrid-filterRowOperationStartsWith": "\u041d\u0430\u0447\u0438\u043d\u0430\u0435\u0442\u0441\u044f \u0441",
            "dxDataGrid-filterRowOperationContains": "\u0421\u043e\u0434\u0435\u0440\u0436\u0438\u0442",
            "dxDataGrid-filterRowOperationNotContains": "\u041d\u0435 \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u0442",
            "dxDataGrid-filterRowOperationEndsWith": "\u0417\u0430\u043a\u0430\u043d\u0447\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u043d\u0430",
            "dxDataGrid-filterRowOperationBetween": "\u0412 \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d\u0435",
            "dxDataGrid-filterRowOperationBetweenStartText": "\u041d\u0430\u0447\u0430\u043b\u043e",
            "dxDataGrid-filterRowOperationBetweenEndText": "\u041a\u043e\u043d\u0435\u0446",
            "dxDataGrid-applyFilterText": "\u041f\u0440\u0438\u043c\u0435\u043d\u0438\u0442\u044c \u0444\u0438\u043b\u044c\u0442\u0440",
            "dxDataGrid-trueText": "\u0414\u0430",
            "dxDataGrid-falseText": "\u041d\u0435\u0442",
            "dxDataGrid-sortingAscendingText": "\u0421\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u043f\u043e \u0432\u043e\u0437\u0440\u0430\u0441\u0442\u0430\u043d\u0438\u044e",
            "dxDataGrid-sortingDescendingText": "\u0421\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u043f\u043e \u0443\u0431\u044b\u0432\u0430\u043d\u0438\u044e",
            "dxDataGrid-sortingClearText": "\u0421\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0441\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u043a\u0443",
            "dxDataGrid-editingSaveAllChanges": "\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f",
            "dxDataGrid-editingCancelAllChanges": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f",
            "dxDataGrid-editingAddRow": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0441\u0442\u0440\u043e\u043a\u0443",
            "dxDataGrid-summaryMin": "\u041c\u0438\u043d: {0}",
            "dxDataGrid-summaryMinOtherColumn": "\u041c\u0438\u043d \u043f\u043e {1} : {0}",
            "dxDataGrid-summaryMax": "\u041c\u0430\u043a\u0441: {0}",
            "dxDataGrid-summaryMaxOtherColumn": "\u041c\u0430\u043a\u0441 \u043f\u043e {1} : {0}",
            "dxDataGrid-summaryAvg": "\u0421\u0440\u0437\u043d\u0430\u0447: {0}",
            "dxDataGrid-summaryAvgOtherColumn": "\u0421\u0440\u0437\u043d\u0430\u0447 \u043f\u043e {1} : {0}",
            "dxDataGrid-summarySum": "\u0421\u0443\u043c\u043c: {0}",
            "dxDataGrid-summarySumOtherColumn": "\u0421\u0443\u043c\u043c \u043f\u043e {1} : {0}",
            "dxDataGrid-summaryCount": "\u041a\u043e\u043b-\u0432\u043e: {0}",
            "dxDataGrid-columnFixingFix": "\u0417\u0430\u043a\u0440\u0435\u043f\u0438\u0442\u044c",
            "dxDataGrid-columnFixingUnfix": "\u041e\u0442\u043a\u0440\u0435\u043f\u0438\u0442\u044c",
            "dxDataGrid-columnFixingLeftPosition": "\u041d\u0430\u043b\u0435\u0432\u043e",
            "dxDataGrid-columnFixingRightPosition": "\u041d\u0430\u043f\u0440\u0430\u0432\u043e",
            "dxDataGrid-exportTo": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c",
            "dxDataGrid-exportToExcel": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432 Excel \u0444\u0430\u0439\u043b",
            "dxDataGrid-exporting": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442...",
            "dxDataGrid-excelFormat": "Excel \u0444\u0430\u0439\u043b",
            "dxDataGrid-selectedRows": "\u0412\u044b\u0431\u0440\u0430\u043d\u043d\u044b\u0435 \u0441\u0442\u0440\u043e\u043a\u0438",
            "dxDataGrid-exportAll": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432\u0441\u0451",
            "dxDataGrid-exportSelectedRows": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432\u044b\u0431\u0440\u0430\u043d\u043d\u044b\u0435 \u0441\u0442\u0440\u043e\u043a\u0438",
            "dxDataGrid-headerFilterEmptyValue": "(\u041f\u0443\u0441\u0442\u043e\u0435)",
            "dxDataGrid-headerFilterOK": "\u041e\u041a",
            "dxDataGrid-headerFilterCancel": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c",
            "dxDataGrid-ariaAdaptiveCollapse": "\u0421\u043a\u0440\u044b\u0442\u044c \u0434\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0435 \u0434\u0430\u043d\u043d\u044b\u0435",
            "dxDataGrid-ariaAdaptiveExpand": "\u041f\u043e\u043a\u0430\u0437\u0430\u0442\u044c \u0434\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0435 \u0434\u0430\u043d\u043d\u044b\u0435",
            "dxDataGrid-ariaColumn": "\u0421\u0442\u043e\u043b\u0431\u0435\u0446",
            "dxDataGrid-ariaValue": "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435",
            "dxDataGrid-ariaFilterCell": "\u0424\u0438\u043b\u044c\u0442\u0440",
            "dxDataGrid-ariaCollapse": "\u0421\u0432\u0435\u0440\u043d\u0443\u0442\u044c",
            "dxDataGrid-ariaExpand": "\u0420\u0430\u0437\u0432\u0435\u0440\u043d\u0443\u0442\u044c",
            "dxDataGrid-ariaDataGrid": "\u0422\u0430\u0431\u043b\u0438\u0446\u0430 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxDataGrid-ariaSearchInGrid": "\u0418\u0441\u043a\u0430\u0442\u044c \u0432 \u0442\u0430\u0431\u043b\u0438\u0446\u0435 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxDataGrid-ariaSelectAll": "\u0412\u044b\u0431\u0440\u0430\u0442\u044c \u0432\u0441\u0451",
            "dxDataGrid-ariaSelectRow": "\u0412\u044b\u0431\u0440\u0430\u0442\u044c \u0441\u0442\u0440\u043e\u043a\u0443",
            "dxDataGrid-ariaToolbar": "\u041f\u0430\u043d\u0435\u043b\u044c \u0438\u043d\u0441\u0442\u0440\u0443\u043c\u0435\u043d\u0442\u043e\u0432 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxDataGrid-filterBuilderPopupTitle": "\u041a\u043e\u043d\u0441\u0442\u0440\u0443\u043a\u0442\u043e\u0440 \u0444\u0438\u043b\u044c\u0442\u0440\u0430",
            "dxDataGrid-filterPanelCreateFilter": "\u0421\u043e\u0437\u0434\u0430\u0442\u044c \u0444\u0438\u043b\u044c\u0442\u0440",
            "dxDataGrid-filterPanelClearFilter": "\u041e\u0447\u0438\u0441\u0442\u0438\u0442\u044c",
            "dxDataGrid-filterPanelFilterEnabledHint": "\u0410\u043a\u0442\u0438\u0432\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0444\u0438\u043b\u044c\u0442\u0440",
            "dxTreeList-ariaTreeList": "\u0418\u0435\u0440\u0430\u0440\u0445\u0438\u0447\u0435\u0441\u043a\u0430\u044f \u0442\u0430\u0431\u043b\u0438\u0446\u0430 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxTreeList-ariaSearchInGrid": "\u0418\u0441\u043a\u0430\u0442\u044c \u0432 \u0438\u0435\u0440\u0430\u0440\u0445\u0438\u0447\u0435\u0441\u043a\u043e\u0439 \u0442\u0430\u0431\u043b\u0438\u0446\u0435 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxTreeList-ariaToolbar": "\u041f\u0430\u043d\u0435\u043b\u044c \u0438\u043d\u0441\u0442\u0440\u0443\u043c\u0435\u043d\u0442\u043e\u0432 \u0438\u0435\u0440\u0430\u0440\u0445\u0438\u0447\u0435\u0441\u043a\u043e\u0439 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxTreeList-editingAddRowToNode": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c",
            "dxPager-infoText": "\u0421\u0442\u0440\u0430\u043d\u0438\u0446\u0430 {0} \u0438\u0437 {1} (\u0412\u0441\u0435\u0433\u043e \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432: {2})",
            "dxPager-pagesCountText": "\u0438\u0437",
            "dxPager-pageSizesAllText": "\u0412\u0441\u0435",
            "dxPivotGrid-grandTotal": "\u0418\u0442\u043e\u0433\u043e",
            "dxPivotGrid-total": "{0} \u0412\u0441\u0435\u0433\u043e",
            "dxPivotGrid-fieldChooserTitle": "\u0412\u044b\u0431\u043e\u0440 \u043f\u043e\u043b\u0435\u0439",
            "dxPivotGrid-showFieldChooser": "\u041f\u043e\u043a\u0430\u0437\u0430\u0442\u044c \u0432\u044b\u0431\u043e\u0440 \u043f\u043e\u043b\u0435\u0439",
            "dxPivotGrid-expandAll": "\u0420\u0430\u0441\u043a\u0440\u044b\u0442\u044c \u0432\u0441\u0435",
            "dxPivotGrid-collapseAll": "\u0421\u0432\u0435\u0440\u043d\u0443\u0442\u044c \u0432\u0441\u0435",
            "dxPivotGrid-sortColumnBySummary": '\u0421\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c "{0}" \u043f\u043e \u044d\u0442\u043e\u0439 \u043a\u043e\u043b\u043e\u043d\u043a\u0435',
            "dxPivotGrid-sortRowBySummary": '\u0421\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c "{0}" \u043f\u043e \u044d\u0442\u043e\u0439 \u0441\u0442\u0440\u043e\u043a\u0435',
            "dxPivotGrid-removeAllSorting": "\u0421\u0431\u0440\u043e\u0441\u0438\u0442\u044c \u0432\u0441\u0435 \u0441\u043e\u0440\u0442\u0438\u0440\u043e\u0432\u043a\u0438",
            "dxPivotGrid-dataNotAvailable": "\u041d/\u0414",
            "dxPivotGrid-rowFields": "\u041f\u043e\u043b\u044f \u0441\u0442\u0440\u043e\u043a",
            "dxPivotGrid-columnFields": "\u041f\u043e\u043b\u044f \u0441\u0442\u043e\u043b\u0431\u0446\u043e\u0432",
            "dxPivotGrid-dataFields": "\u041f\u043e\u043b\u044f \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxPivotGrid-filterFields": "\u041f\u043e\u043b\u044f \u0444\u0438\u043b\u044c\u0442\u0440\u043e\u0432",
            "dxPivotGrid-allFields": "\u0412\u0441\u0435 \u043f\u043e\u043b\u044f",
            "dxPivotGrid-columnFieldArea": "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u043f\u043e\u043b\u044f \u043a\u043e\u043b\u043e\u043d\u043e\u043a c\u044e\u0434\u0430",
            "dxPivotGrid-dataFieldArea": "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u043f\u043e\u043b\u044f \u0434\u0430\u043d\u043d\u044b\u0445 c\u044e\u0434\u0430",
            "dxPivotGrid-rowFieldArea": "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u043f\u043e\u043b\u044f \u0441\u0442\u0440\u043e\u043a c\u044e\u0434\u0430",
            "dxPivotGrid-filterFieldArea": "\u041f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u043f\u043e\u043b\u044f \u0444\u0438\u043b\u044c\u0442\u0440\u043e\u0432 c\u044e\u0434\u0430",
            "dxScheduler-editorLabelTitle": "\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435",
            "dxScheduler-editorLabelStartDate": "\u0414\u0430\u0442\u0430 \u043d\u0430\u0447\u0430\u043b\u0430",
            "dxScheduler-editorLabelEndDate": "\u0414\u0430\u0442\u0430 \u0437\u0430\u0432\u0435\u0440\u0448\u0435\u043d\u0438\u044f",
            "dxScheduler-editorLabelDescription": "\u041e\u043f\u0438\u0441\u0430\u043d\u0438\u0435",
            "dxScheduler-editorLabelRecurrence": "\u041f\u043e\u0432\u0442\u043e\u0440\u0435\u043d\u0438\u0435",
            "dxScheduler-openAppointment": "\u041e\u0442\u043a\u0440\u044b\u0442\u044c \u0437\u0430\u0434\u0430\u0447\u0443",
            "dxScheduler-recurrenceNever": "\u041d\u0438\u043a\u043e\u0433\u0434\u0430",
            "dxScheduler-recurrenceMinutely": "\u0415\u0436\u0435\u043c\u0438\u043d\u0443\u0442\u043d\u043e",
            "dxScheduler-recurrenceHourly": "\u0415\u0436\u0435\u0447\u0430\u0441\u043d\u043e",
            "dxScheduler-recurrenceDaily": "\u0415\u0436\u0435\u0434\u043d\u0435\u0432\u043d\u043e",
            "dxScheduler-recurrenceWeekly": "\u0415\u0436\u0435\u043d\u0435\u0434\u0435\u043b\u044c\u043d\u043e",
            "dxScheduler-recurrenceMonthly": "\u0415\u0436\u0435\u043c\u0435\u0441\u044f\u0447\u043d\u043e",
            "dxScheduler-recurrenceYearly": "\u0415\u0436\u0435\u0433\u043e\u0434\u043d\u043e",
            "dxScheduler-recurrenceRepeatEvery": "\u0418\u043d\u0442\u0435\u0440\u0432\u0430\u043b",
            "dxScheduler-recurrenceRepeatOn": "\u041f\u043e\u0432\u0442\u043e\u0440\u044f\u0442\u044c \u043f\u043e",
            "dxScheduler-recurrenceEnd": "\u0417\u0430\u0432\u0435\u0440\u0448\u0438\u0442\u044c \u043f\u043e\u0432\u0442\u043e\u0440\u0435\u043d\u0438\u0435",
            "dxScheduler-recurrenceAfter": "\u041f\u043e\u0441\u043b\u0435",
            "dxScheduler-recurrenceOn": "\u041f\u043e\u0432\u0442\u043e\u0440\u044f\u0442\u044c \u0434\u043e",
            "dxScheduler-recurrenceRepeatMinutely": "\u043c\u0438\u043d\u0443\u0442(\u043c\u0438\u043d\u0443\u0442\u044b)",
            "dxScheduler-recurrenceRepeatHourly": "\u0447\u0430\u0441\u043e\u0432(\u0447\u0430\u0441\u0430)",
            "dxScheduler-recurrenceRepeatDaily": "\u0434\u043d\u0435\u0439(\u0434\u043d\u044f)",
            "dxScheduler-recurrenceRepeatWeekly": "\u043d\u0435\u0434\u0435\u043b\u0438(\u043d\u0435\u0434\u0435\u043b\u044c)",
            "dxScheduler-recurrenceRepeatMonthly": "\u043c\u0435\u0441\u044f\u0446\u0430(\u043c\u0435\u0441\u044f\u0446\u0435\u0432)",
            "dxScheduler-recurrenceRepeatYearly": "\u0433\u043e\u0434\u0430(\u043b\u0435\u0442)",
            "dxScheduler-recurrenceRepeatOnDate": "\u0434\u043e \u0434\u0430\u0442\u044b",
            "dxScheduler-recurrenceRepeatCount": "\u043f\u043e\u0432\u0442\u043e\u0440\u0435\u043d\u0438\u0439",
            "dxScheduler-switcherDay": "\u0414\u0435\u043d\u044c",
            "dxScheduler-switcherWeek": "\u041d\u0435\u0434\u0435\u043b\u044f",
            "dxScheduler-switcherWorkWeek": "\u0420\u0430\u0431\u043e\u0447\u0430\u044f \u043d\u0435\u0434\u0435\u043b\u044f",
            "dxScheduler-switcherMonth": "\u041c\u0435\u0441\u044f\u0446",
            "dxScheduler-switcherTimelineDay": "\u0425\u0440\u043e\u043d\u043e\u043b\u043e\u0433\u0438\u044f \u0434\u043d\u044f",
            "dxScheduler-switcherTimelineWeek": "\u0425\u0440\u043e\u043d\u043e\u043b\u043e\u0433\u0438\u044f \u043d\u0435\u0434\u0435\u043b\u0438",
            "dxScheduler-switcherTimelineWorkWeek": "\u0425\u0440\u043e\u043d\u043e\u043b\u043e\u0433\u0438\u044f \u0440\u0430\u0431\u043e\u0447\u0435\u0439 \u043d\u0435\u0434\u0435\u043b\u0438",
            "dxScheduler-switcherTimelineMonth": "\u0425\u0440\u043e\u043d\u043e\u043b\u043e\u0433\u0438\u044f \u043c\u0435\u0441\u044f\u0446\u0430",
            "dxScheduler-switcherAgenda": "\u0420\u0430\u0441\u043f\u0438\u0441\u0430\u043d\u0438\u0435",
            "dxScheduler-allDay": "\u0412\u0435\u0441\u044c \u0434\u0435\u043d\u044c",
            "dxScheduler-confirmRecurrenceEditMessage": "\u0412\u044b \u0445\u043e\u0442\u0438\u0442\u0435 \u043e\u0442\u0440\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0442\u043e\u043b\u044c\u043a\u043e \u044d\u0442\u043e \u0441\u043e\u0431\u044b\u0442\u0438\u0435 \u0438\u043b\u0438 \u0432\u0441\u044e \u0441\u0435\u0440\u0438\u044e?",
            "dxScheduler-confirmRecurrenceDeleteMessage": "\u0412\u044b \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c \u0442\u043e\u043b\u044c\u043a\u043e \u044d\u0442\u043e \u0441\u043e\u0431\u044b\u0442\u0438\u0435 \u0438\u043b\u0438 \u0432\u0441\u044e \u0441\u0435\u0440\u0438\u044e?",
            "dxScheduler-confirmRecurrenceEditSeries": "\u0412\u0441\u044e \u0441\u0435\u0440\u0438\u044e",
            "dxScheduler-confirmRecurrenceDeleteSeries": "\u0412\u0441\u044e \u0441\u0435\u0440\u0438\u044e",
            "dxScheduler-confirmRecurrenceEditOccurrence": "\u0422\u043e\u043b\u044c\u043a\u043e \u044d\u0442\u043e \u0441\u043e\u0431\u044b\u0442\u0438\u0435",
            "dxScheduler-confirmRecurrenceDeleteOccurrence": "\u0422\u043e\u043b\u044c\u043a\u043e \u044d\u0442\u043e \u0441\u043e\u0431\u044b\u0442\u0438\u0435",
            "dxScheduler-noTimezoneTitle": "\u0427\u0430\u0441\u043e\u0432\u043e\u0439 \u043f\u043e\u044f\u0441 \u043d\u0435 \u0432\u044b\u0431\u0440\u0430\u043d",
            "dxScheduler-moreAppointments": "\u0438 \u0435\u0449\u0435 {0}",
            "dxCalendar-todayButtonText": "\u0421\u0435\u0433\u043e\u0434\u043d\u044f",
            "dxCalendar-ariaWidgetName": "\u041a\u0430\u043b\u0435\u043d\u0434\u0430\u0440\u044c",
            "dxColorView-ariaRed": "\u041a\u0440\u0430\u0441\u043d\u044b\u0439",
            "dxColorView-ariaGreen": "\u0417\u0435\u043b\u0435\u043d\u044b\u0439",
            "dxColorView-ariaBlue": "\u0421\u0438\u043d\u0438\u0439",
            "dxColorView-ariaAlpha": "\u041f\u0440\u043e\u0437\u0440\u0430\u0447\u043d\u043e\u0441\u0442\u044c",
            "dxColorView-ariaHex": "\u041a\u043e\u0434 \u0446\u0432\u0435\u0442\u0430",
            "dxTagBox-selected": "{0} \u0432\u044b\u0431\u0440\u0430\u043d\u043e",
            "dxTagBox-allSelected": "\u0412\u044b\u0431\u0440\u0430\u043d\u043e \u0432\u0441\u0435 ({0})",
            "dxTagBox-moreSelected": "\u0438 \u0435\u0449\u0435 {0}",
            "vizExport-printingButtonText": "\u041f\u0435\u0447\u0430\u0442\u044c",
            "vizExport-titleMenuText": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442/\u041f\u0435\u0447\u0430\u0442\u044c",
            "vizExport-exportButtonText": "{0} \u0444\u0430\u0439\u043b",
            "dxFilterBuilder-and": "\u0418",
            "dxFilterBuilder-or": "\u0418\u043b\u0438",
            "dxFilterBuilder-notAnd": "\u041d\u0435 \u0418",
            "dxFilterBuilder-notOr": "\u041d\u0435 \u0418\u043b\u0438",
            "dxFilterBuilder-addCondition": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0443\u0441\u043b\u043e\u0432\u0438\u0435",
            "dxFilterBuilder-addGroup": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0433\u0440\u0443\u043f\u043f\u0443",
            "dxFilterBuilder-enterValueText": "<\u0432\u0432\u0435\u0434\u0438\u0442\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435>",
            "dxFilterBuilder-filterOperationEquals": "\u0420\u0430\u0432\u043d\u043e",
            "dxFilterBuilder-filterOperationNotEquals": "\u041d\u0435 \u0440\u0430\u0432\u043d\u043e",
            "dxFilterBuilder-filterOperationLess": "\u041c\u0435\u043d\u044c\u0448\u0435",
            "dxFilterBuilder-filterOperationLessOrEquals": "\u041c\u0435\u043d\u044c\u0448\u0435 \u0438\u043b\u0438 \u0440\u0430\u0432\u043d\u043e",
            "dxFilterBuilder-filterOperationGreater": "\u0411\u043e\u043b\u044c\u0448\u0435",
            "dxFilterBuilder-filterOperationGreaterOrEquals": "\u0411\u043e\u043b\u044c\u0448\u0435 \u0438\u043b\u0438 \u0440\u0430\u0432\u043d\u043e",
            "dxFilterBuilder-filterOperationStartsWith": "\u041d\u0430\u0447\u0438\u043d\u0430\u0435\u0442\u0441\u044f \u0441",
            "dxFilterBuilder-filterOperationContains": "\u0421\u043e\u0434\u0435\u0440\u0436\u0438\u0442",
            "dxFilterBuilder-filterOperationNotContains": "\u041d\u0435 \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u0442",
            "dxFilterBuilder-filterOperationEndsWith": "\u0417\u0430\u043a\u0430\u043d\u0447\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u043d\u0430",
            "dxFilterBuilder-filterOperationIsBlank": "\u041f\u0443\u0441\u0442\u043e",
            "dxFilterBuilder-filterOperationIsNotBlank": "\u041d\u0435 \u043f\u0443\u0441\u0442\u043e",
            "dxFilterBuilder-filterOperationBetween": "\u0412 \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d\u0435",
            "dxFilterBuilder-filterOperationAnyOf": "\u041b\u044e\u0431\u043e\u0439 \u0438\u0437",
            "dxFilterBuilder-filterOperationNoneOf": "\u041d\u0438 \u043e\u0434\u0438\u043d \u0438\u0437",
            "dxHtmlEditor-dialogColorCaption": "\u0418\u0437\u043c\u0435\u043d\u0438\u0442\u044c \u0446\u0432\u0435\u0442 \u0442\u0435\u043a\u0441\u0442\u0430",
            "dxHtmlEditor-dialogBackgroundCaption": "\u0418\u0437\u043c\u0435\u043d\u0438\u0442\u044c \u0446\u0432\u0435\u0442 \u0444\u043e\u043d\u0430",
            "dxHtmlEditor-dialogLinkCaption": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0441\u0441\u044b\u043b\u043a\u0443",
            "dxHtmlEditor-dialogLinkUrlField": "URL",
            "dxHtmlEditor-dialogLinkTextField": "\u0422\u0435\u043a\u0441\u0442",
            "dxHtmlEditor-dialogLinkTargetField": "\u041e\u0442\u043a\u0440\u044b\u0442\u044c \u0432 \u043d\u043e\u0432\u043e\u043c \u043e\u043a\u043d\u0435",
            "dxHtmlEditor-dialogImageCaption": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435",
            "dxHtmlEditor-dialogImageUrlField": "URL",
            "dxHtmlEditor-dialogImageAltField": "\u0410\u043b\u044c\u0442\u0435\u0440\u043d\u0430\u0442\u0438\u0432\u043d\u044b\u0439 \u0442\u0435\u043a\u0441\u0442",
            "dxHtmlEditor-dialogImageWidthField": "\u0428\u0438\u0440\u0438\u043d\u0430 (px)",
            "dxHtmlEditor-dialogImageHeightField": "\u0412\u044b\u0441\u043e\u0442\u0430 (px)",
            "dxHtmlEditor-dialogInsertTableRowsField": "\u0421\u0442\u0440\u043e\u043a\u0438",
            "dxHtmlEditor-dialogInsertTableColumnsField": "\u041a\u043e\u043b\u043e\u043d\u043a\u0438",
            "dxHtmlEditor-dialogInsertTableCaption": "\u0412\u0441\u0442\u0430\u0432\u0438\u0442\u044c \u0442\u0430\u0431\u043b\u0438\u0446\u0443",
            "dxHtmlEditor-heading": "\u0417\u0430\u0433\u043e\u043b\u043e\u0432\u043e\u043a",
            "dxHtmlEditor-normalText": "\u041e\u0431\u044b\u0447\u043d\u044b\u0439 \u0442\u0435\u043a\u0441\u0442",
            "dxFileManager-newDirectoryName": "\u0411\u0435\u0437 \u043d\u0430\u0437\u0432\u0430\u043d\u0438\u044f",
            "dxFileManager-rootDirectoryName": "\u0424\u0430\u0439\u043b\u044b",
            "dxFileManager-errorNoAccess": "\u0414\u043e\u0441\u0442\u0443\u043f \u0437\u0430\u043f\u0440\u0435\u0449\u0451\u043d. \u041e\u043f\u0435\u0440\u0430\u0446\u0438\u044f \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u0437\u0430\u0432\u0435\u0440\u0448\u0435\u043d\u0430.",
            "dxFileManager-errorDirectoryExistsFormat": "\u041a\u0430\u0442\u0430\u043b\u043e\u0433 {0} \u0443\u0436\u0435 \u0441\u0443\u0449\u0435\u0441\u0442\u0432\u0443\u0435\u0442.",
            "dxFileManager-errorFileExistsFormat": "\u0424\u0430\u0439\u043b {0} \u0443\u0436\u0435 \u0441\u0443\u0449\u0435\u0441\u0442\u0432\u0443\u0435\u0442.",
            "dxFileManager-errorFileNotFoundFormat": "\u0424\u0430\u0439\u043b {0} \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d.",
            "dxFileManager-errorDirectoryNotFoundFormat": "\u041a\u0430\u0442\u0430\u043b\u043e\u0433 '{0}' \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d.",
            "dxFileManager-errorWrongFileExtension": "\u041d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0435 \u0444\u0430\u0439\u043b\u0430.",
            "dxFileManager-errorMaxFileSizeExceeded": "\u0420\u0430\u0437\u043c\u0435\u0440 \u0444\u0430\u0439\u043b\u0430 \u043f\u0440\u0435\u0432\u044b\u0448\u0430\u0435\u0442 \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435.",
            "dxFileManager-errorInvalidSymbols": "\u0412\u0432\u0435\u0434\u0451\u043d\u043d\u043e\u0435 \u0438\u043c\u044f \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u0442 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0435 \u0441\u0438\u043c\u0432\u043e\u043b\u044b.",
            "dxFileManager-errorDefault": "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0430\u044f \u043e\u0448\u0438\u0431\u043a\u0430",
            "dxFileManager-errorDirectoryOpenFailed": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043e\u0442\u043a\u0440\u044b\u0442\u044c \u043a\u0430\u0442\u0430\u043b\u043e\u0433",
            "dxFileManager-commandCreate": "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430",
            "dxFileManager-commandRename": "\u041f\u0435\u0440\u0435\u0438\u043c\u0435\u043d\u043e\u0432\u0430\u0442\u044c",
            "dxFileManager-commandMove": "\u041f\u0435\u0440\u0435\u043c\u0435\u0441\u0442\u0438\u0442\u044c \u0432",
            "dxFileManager-commandCopy": "\u041a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432",
            "dxFileManager-commandDelete": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            "dxFileManager-commandDownload": "\u0421\u043a\u0430\u0447\u0430\u0442\u044c",
            "dxFileManager-commandUpload": "\u0417\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c \u0444\u0430\u0439\u043b\u044b",
            "dxFileManager-commandRefresh": "\u041e\u0431\u043d\u043e\u0432\u0438\u0442\u044c",
            "dxFileManager-commandThumbnails": "\u0420\u0435\u0436\u0438\u043c \u044d\u043a\u0441\u0438\u0437\u043e\u0432",
            "dxFileManager-commandDetails": "\u0420\u0435\u0436\u0438\u043c \u0441\u043f\u0438\u0441\u043a\u0430",
            "dxFileManager-commandClearSelection": "\u041e\u0447\u0438\u0441\u0442\u0438\u0442\u044c \u0432\u044b\u0434\u0435\u043b\u0435\u043d\u0438\u0435",
            "dxFileManager-dialogButtonCancel": "\u041e\u0442\u043c\u0435\u043d\u0430",
            "dxFileManager-commandShowNavPane": "\u041f\u0435\u0440\u0435\u043a\u043b\u044e\u0447\u0438\u0442\u044c \u043f\u0430\u043d\u0435\u043b\u044c \u043d\u0430\u0432\u0438\u0433\u0430\u0446\u0438\u0438",
            "dxFileManager-dialogDirectoryChooserMoveTitle": "\u041f\u0435\u0440\u0435\u043c\u0435\u0441\u0442\u0438\u0442\u044c \u0432",
            "dxFileManager-dialogDirectoryChooserMoveButtonText": "\u041f\u0435\u0440\u0435\u043c\u0435\u0441\u0442\u0438\u0442\u044c",
            "dxFileManager-dialogDirectoryChooserCopyTitle": "\u041a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432",
            "dxFileManager-dialogDirectoryChooserCopyButtonText": "\u041a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c",
            "dxFileManager-dialogRenameItemTitle": "\u041f\u0435\u0440\u0435\u0438\u043c\u0435\u043d\u043e\u0432\u0430\u0442\u044c",
            "dxFileManager-dialogRenameItemButtonText": "\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c",
            "dxFileManager-dialogCreateDirectoryTitle": "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430",
            "dxFileManager-dialogCreateDirectoryButtonText": "\u0421\u043e\u0437\u0434\u0430\u0442\u044c",
            "dxFileManager-dialogDeleteItemTitle": "\u0423\u0434\u0430\u043b\u0435\u043d\u0438\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430",
            "dxFileManager-dialogDeleteItemButtonText": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            "dxFileManager-dialogDeleteItemSingleItemConfirmation": "\u0412\u044b \u0434\u0435\u0439\u0441\u0442\u0432\u0438\u0442\u0435\u043b\u044c\u043d\u043e \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c {0}?",
            "dxFileManager-dialogDeleteItemMultipleItemsConfirmation": "\u0412\u044b \u0434\u0435\u0439\u0441\u0442\u0432\u0438\u0442\u0435\u043b\u044c\u043d\u043e \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c {0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432?",
            "dxFileManager-editingCreateSingleItemProcessingMessage": "\u0421\u043e\u0437\u0434\u0430\u0451\u0442\u0441\u044f \u043f\u0430\u043f\u043a\u0430 \u0432 {0}",
            "dxFileManager-editingCreateSingleItemSuccessMessage": "\u0421\u043e\u0437\u0434\u0430\u043d\u0430 \u043f\u0430\u043f\u043a\u0430 \u0432 {0}",
            "dxFileManager-editingCreateSingleItemErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u043f\u0430\u043f\u043a\u0443",
            "dxFileManager-editingCreateCommonErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u043f\u0430\u043f\u043a\u0443",
            "dxFileManager-editingRenameSingleItemProcessingMessage": "\u041f\u0435\u0440\u0435\u0438\u043c\u0435\u043d\u043e\u0432\u044b\u0432\u0430\u0435\u0442\u0441\u044f \u044d\u043b\u0435\u043c\u0435\u043d\u0442 \u0432 {0}",
            "dxFileManager-editingRenameSingleItemSuccessMessage": "\u041f\u0435\u0440\u0435\u0438\u043c\u0435\u043d\u043e\u0432\u0430\u043d \u044d\u043b\u0435\u043c\u0435\u043d\u0442 \u0432 {0}",
            "dxFileManager-editingRenameSingleItemErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0435\u0440\u0435\u0438\u043c\u0435\u043d\u043e\u0432\u0430\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442",
            "dxFileManager-editingRenameCommonErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0435\u0440\u0435\u0438\u043c\u0435\u043d\u043e\u0432\u0430\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442",
            "dxFileManager-editingDeleteSingleItemProcessingMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u0443\u0434\u0430\u043b\u044f\u0435\u0442\u0441\u044f \u0438\u0437 {0}",
            "dxFileManager-editingDeleteMultipleItemsProcessingMessage": "\u0423\u0434\u0430\u043b\u0435\u043d\u0438\u0435 {0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0438\u0437 {1}",
            "dxFileManager-editingDeleteSingleItemSuccessMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u0443\u0434\u0430\u043b\u0451\u043d \u0438\u0437 {0}",
            "dxFileManager-editingDeleteMultipleItemsSuccessMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0443\u0434\u0430\u043b\u0435\u043d\u043e \u0438\u0437 {1}",
            "dxFileManager-editingDeleteSingleItemErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0443\u0434\u0430\u043b\u0438\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442",
            "dxFileManager-editingDeleteMultipleItemsErrorMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u043d\u0435 \u0431\u044b\u043b\u0438 \u0443\u0434\u0430\u043b\u0435\u043d\u044b",
            "dxFileManager-editingDeleteCommonErrorMessage": "\u041d\u0435\u043a\u043e\u0442\u043e\u0440\u044b\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u044b \u043d\u0435 \u0431\u044b\u043b\u0438 \u0443\u0434\u0430\u043b\u0435\u043d\u044b",
            "dxFileManager-editingMoveSingleItemProcessingMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u043f\u0435\u0440\u0435\u043c\u0435\u0449\u0430\u0435\u0442\u0441\u044f \u0432 {0}",
            "dxFileManager-editingMoveMultipleItemsProcessingMessage": "\u041f\u0435\u0440\u0435\u043c\u0435\u0449\u0435\u043d\u0438\u0435 {0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0432 {1}",
            "dxFileManager-editingMoveSingleItemSuccessMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u043f\u0435\u0440\u0435\u043c\u0435\u0449\u0451\u043d \u0432 {0}",
            "dxFileManager-editingMoveMultipleItemsSuccessMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u043f\u0435\u0440\u0435\u043c\u0435\u0449\u0435\u043d\u043e \u0432 {1}",
            "dxFileManager-editingMoveSingleItemErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0435\u0440\u0435\u043c\u0435\u0441\u0442\u0438\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442",
            "dxFileManager-editingMoveMultipleItemsErrorMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u043d\u0435 \u0431\u044b\u043b\u0438 \u043f\u0435\u0440\u0435\u043c\u0435\u0449\u0435\u043d\u044b",
            "dxFileManager-editingMoveCommonErrorMessage": "\u041d\u0435\u043a\u043e\u0442\u043e\u0440\u044b\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u044b \u043d\u0435 \u0431\u044b\u043b\u0438 \u043f\u0435\u0440\u0435\u043c\u0435\u0449\u0435\u043d\u044b",
            "dxFileManager-editingCopySingleItemProcessingMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u043a\u043e\u043f\u0438\u0440\u0443\u0435\u0442\u0441\u044f \u0432 {0}",
            "dxFileManager-editingCopyMultipleItemsProcessingMessage": "\u041a\u043e\u0438\u043f\u0440\u043e\u0432\u0430\u043d\u0438\u0435 {0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0432 {1}",
            "dxFileManager-editingCopySingleItemSuccessMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d \u0432 {0}",
            "dxFileManager-editingCopyMultipleItemsSuccessMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d\u043e \u0432 {1}",
            "dxFileManager-editingCopySingleItemErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442",
            "dxFileManager-editingCopyMultipleItemsErrorMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u043d\u0435 \u0431\u044b\u043b\u0438 \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d\u044b",
            "dxFileManager-editingCopyCommonErrorMessage": "\u041d\u0435\u043a\u043e\u0442\u043e\u0440\u044b\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u044b \u043d\u0435 \u0431\u044b\u043b\u0438 \u0441\u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d\u044b",
            "dxFileManager-editingUploadSingleItemProcessingMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u0437\u0430\u0433\u0440\u0443\u0436\u0430\u0435\u0442\u0441\u044f \u0432 {0}",
            "dxFileManager-editingUploadMultipleItemsProcessingMessage": "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430 {0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0432 {1}",
            "dxFileManager-editingUploadSingleItemSuccessMessage": "\u042d\u043b\u0435\u043c\u0435\u043d\u0442 \u0437\u0430\u0433\u0440\u0443\u0436\u0435\u043d \u0432 {0}",
            "dxFileManager-editingUploadMultipleItemsSuccessMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u0437\u0430\u0433\u0440\u0443\u0436\u0435\u043d\u043e \u0432 {1}",
            "dxFileManager-editingUploadSingleItemErrorMessage": "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0437\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442",
            "dxFileManager-editingUploadMultipleItemsErrorMessage": "{0} \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u0432 \u043d\u0435 \u0431\u044b\u043b\u0438 \u0437\u0430\u0433\u0440\u0443\u0436\u0435\u043d\u044b",
            "dxFileManager-editingUploadCanceledMessage": "\u041e\u0442\u043c\u0435\u043d\u0435\u043d\u043e",
            "dxFileManager-listDetailsColumnCaptionName": "\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435",
            "dxFileManager-listDetailsColumnCaptionDateModified": "\u0414\u0430\u0442\u0430 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f",
            "dxFileManager-listDetailsColumnCaptionFileSize": "\u0420\u0430\u0437\u043c\u0435\u0440 \u0444\u0430\u0439\u043b\u0430",
            "dxFileManager-listThumbnailsTooltipTextSize": "\u0420\u0430\u0437\u043c\u0435\u0440",
            "dxFileManager-listThumbnailsTooltipTextDateModified": "\u0414\u0430\u0442\u0430 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f",
            "dxFileManager-notificationProgressPanelTitle": "\u041f\u0440\u043e\u0433\u0440\u0435\u0441\u0441",
            "dxFileManager-notificationProgressPanelEmptyListText": "\u041e\u043f\u0435\u0440\u0430\u0446\u0438\u0438 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u044e\u0442",
            "dxFileManager-notificationProgressPanelOperationCanceled": "\u041e\u0442\u043c\u0435\u043d\u0435\u043d\u043e",
            "dxDiagram-categoryGeneral": "\u041e\u0431\u0449\u0438\u0435",
            "dxDiagram-categoryFlowchart": "\u0411\u043b\u043e\u043a-\u0441\u0445\u0435\u043c\u0430",
            "dxDiagram-categoryOrgChart": "\u041e\u0440\u0433\u0430\u043d\u0438\u0437\u0430\u0446\u0438\u043e\u043d\u043d\u0430\u044f \u0441\u0445\u0435\u043c\u0430",
            "dxDiagram-categoryContainers": "\u041a\u043e\u043d\u0442\u0435\u0439\u043d\u0435\u0440\u044b",
            "dxDiagram-categoryCustom": "\u041f\u043e\u043b\u044c\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u044c\u0441\u043a\u0438\u0435",
            "dxDiagram-commandExportToSvg": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442 \u0432 SVG",
            "dxDiagram-commandExportToPng": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442 \u0432 PNG",
            "dxDiagram-commandExportToJpg": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442 \u0432 JPEG",
            "dxDiagram-commandUndo": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c",
            "dxDiagram-commandRedo": "\u041f\u043e\u0432\u0442\u043e\u0440\u0438\u0442\u044c",
            "dxDiagram-commandFontName": "\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435 \u0448\u0440\u0438\u0444\u0442\u0430",
            "dxDiagram-commandFontSize": "\u0420\u0430\u0437\u043c\u0435\u0440 \u0448\u0440\u0438\u0444\u0442\u0430",
            "dxDiagram-commandBold": "\u041f\u043e\u043b\u0443\u0436\u0438\u0440\u043d\u044b\u0439",
            "dxDiagram-commandItalic": "\u041a\u0443\u0440\u0441\u0438\u0432",
            "dxDiagram-commandUnderline": "\u041f\u043e\u0434\u0447\u0435\u0440\u043a\u043d\u0443\u0442\u044b\u0439",
            "dxDiagram-commandTextColor": "\u0426\u0432\u0435\u0442 \u0442\u0435\u043a\u0441\u0442\u0430",
            "dxDiagram-commandLineColor": "\u0426\u0432\u0435\u0442 \u043b\u0438\u043d\u0438\u0438",
            "dxDiagram-commandLineWidth": "\u0428\u0438\u0440\u0438\u043d\u0430 \u043b\u0438\u043d\u0438\u0438",
            "dxDiagram-commandLineStyle": "\u0421\u0442\u0438\u043b\u044c \u043b\u0438\u043d\u0438\u0438",
            "dxDiagram-commandLineStyleSolid": "\u0421\u043f\u043b\u043e\u0448\u043d\u0430\u044f",
            "dxDiagram-commandLineStyleDotted": "\u041f\u0443\u043d\u043a\u0442\u0438\u0440\u043d\u0430\u044f",
            "dxDiagram-commandLineStyleDashed": "\u0428\u0442\u0440\u0438\u0445\u043e\u0432\u0430\u044f",
            "dxDiagram-commandFillColor": "\u0426\u0432\u0435\u0442 \u0437\u0430\u043b\u0438\u0432\u043a\u0438",
            "dxDiagram-commandAlignLeft": "\u0412\u044b\u0440\u0430\u0432\u043d\u0438\u0432\u0430\u043d\u0438\u0435 \u043f\u043e \u043b\u0435\u0432\u043e\u043c\u0443 \u043a\u0440\u0430\u044e",
            "dxDiagram-commandAlignCenter": "\u0412\u044b\u0440\u0430\u0432\u043d\u0438\u0432\u0430\u043d\u0438\u0435 \u043f\u043e \u0446\u0435\u043d\u0442\u0440\u0443",
            "dxDiagram-commandAlignRight": "\u0412\u044b\u0440\u0430\u0432\u043d\u0438\u0432\u0430\u043d\u0438\u0435 \u043f\u043e \u043f\u0440\u0430\u0432\u043e\u043c\u0443 \u043a\u0440\u0430\u044e",
            "dxDiagram-commandConnectorLineType": "\u0422\u0438\u043f \u0441\u043e\u0435\u0434\u0438\u043d\u0438\u0442\u0435\u043b\u044f",
            "dxDiagram-commandConnectorLineStraight": "\u041f\u0440\u044f\u043c\u043e\u0439",
            "dxDiagram-commandConnectorLineOrthogonal": "\u041e\u0440\u0442\u043e\u0433\u043e\u043d\u0430\u043b\u044c\u043d\u044b\u0439",
            "dxDiagram-commandConnectorLineStart": "\u0422\u0438\u043f \u043d\u0430\u0447\u0430\u043b\u0430 \u0441\u043e\u0435\u0434\u0438\u043d\u0438\u0442\u0435\u043b\u044f",
            "dxDiagram-commandConnectorLineEnd": "\u0422\u0438\u043f \u043a\u043e\u043d\u0446\u0430 \u0441\u043e\u0435\u0434\u0438\u043d\u0438\u0442\u0435\u043b\u044f",
            "dxDiagram-commandConnectorLineNone": "\u041d\u0435\u0442",
            "dxDiagram-commandConnectorLineArrow": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430",
            "dxDiagram-commandFullscreen": "\u041f\u043e\u043b\u043d\u043e\u044d\u043a\u0440\u0430\u043d\u043d\u044b\u0439 \u0440\u0435\u0436\u0438\u043c",
            "dxDiagram-commandUnits": "\u0415\u0434\u0438\u043d\u0438\u0446\u044b \u0438\u0437\u043c\u0435\u0440\u0435\u043d\u0438\u044f",
            "dxDiagram-commandPageSize": "\u0420\u0430\u0437\u043c\u0435\u0440 \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u044b",
            "dxDiagram-commandPageOrientation": "\u041e\u0440\u0438\u0435\u043d\u0442\u0430\u0446\u0438\u044f \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u044b",
            "dxDiagram-commandPageOrientationLandscape": "\u0410\u043b\u044c\u0431\u043e\u043c\u043d\u0430\u044f",
            "dxDiagram-commandPageOrientationPortrait": "\u041f\u043e\u0440\u0442\u0440\u0435\u0442\u043d\u0430\u044f",
            "dxDiagram-commandPageColor": "\u0426\u0432\u0435\u0442 \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u044b",
            "dxDiagram-commandShowGrid": "\u041f\u043e\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u0441\u0435\u0442\u043a\u0443",
            "dxDiagram-commandSnapToGrid": "\u041f\u0440\u0438\u0432\u044f\u0437\u043a\u0430 \u043a \u0441\u0435\u0442\u043a\u0435",
            "dxDiagram-commandGridSize": "\u0420\u0430\u0437\u043c\u0435\u0440 \u0441\u0435\u0442\u043a\u0438",
            "dxDiagram-commandZoomLevel": "\u041c\u0430\u0448\u0442\u0430\u0431",
            "dxDiagram-commandAutoZoom": "\u0410\u0432\u0442\u043e\u043c\u0430\u0448\u0442\u0430\u0431",
            "dxDiagram-commandFitToContent": "\u0412\u043c\u0435\u0441\u0442\u0438\u0442\u044c \u043f\u043e \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u043c\u043e\u043c\u0443",
            "dxDiagram-commandFitToWidth": "\u0412\u043c\u0435\u0441\u0442\u0438\u0442\u044c \u043f\u043e \u0448\u0438\u0440\u0438\u043d\u0435",
            "dxDiagram-commandAutoZoomByContent": "\u0410\u0432\u0442\u043e\u043c\u0430\u0448\u0442\u0430\u0431 \u043f\u043e \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u043c\u043e\u043c\u0443",
            "dxDiagram-commandAutoZoomByWidth": "\u0410\u0432\u0442\u043e\u043c\u0430\u0448\u0442\u0430\u0431 \u043f\u043e \u0448\u0438\u0440\u0438\u043d\u0435",
            "dxDiagram-commandSimpleView": "\u0423\u043f\u0440\u043e\u0449\u0435\u043d\u043d\u044b\u0439 \u0432\u0438\u0434",
            "dxDiagram-commandCut": "\u0412\u044b\u0440\u0435\u0437\u0430\u0442\u044c",
            "dxDiagram-commandCopy": "\u041a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c",
            "dxDiagram-commandPaste": "\u0412\u0441\u0442\u0430\u0432\u0438\u0442\u044c",
            "dxDiagram-commandSelectAll": "\u0412\u044b\u0434\u0435\u043b\u0438\u0442\u044c \u0432\u0441\u0451",
            "dxDiagram-commandDelete": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            "dxDiagram-commandBringToFront": "\u041d\u0430 \u043f\u0435\u0440\u0435\u0434\u043d\u0438\u0439 \u043f\u043b\u0430\u043d",
            "dxDiagram-commandSendToBack": "\u041d\u0430 \u0437\u0430\u0434\u043d\u0438\u0439 \u043f\u043b\u0430\u043d",
            "dxDiagram-commandLock": "\u0417\u0430\u0431\u043b\u043e\u043a\u0438\u0440\u043e\u0432\u0430\u0442\u044c",
            "dxDiagram-commandUnlock": "\u0420\u0430\u0437\u0431\u043b\u043e\u043a\u0438\u0440\u043e\u0432\u0430\u0442\u044c",
            "dxDiagram-commandInsertShapeImage": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435...",
            "dxDiagram-commandEditShapeImage": "\u0418\u0437\u043c\u0435\u043d\u0438\u0442\u044c \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435...",
            "dxDiagram-commandDeleteShapeImage": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435",
            "dxDiagram-commandLayoutLeftToRight": "\u0421\u043b\u0435\u0432\u0430 \u043d\u0430\u043f\u0440\u0430\u0432\u043e",
            "dxDiagram-commandLayoutRightToLeft": "\u0421\u043f\u0440\u0430\u0432\u0430 \u043d\u0430\u043b\u0435\u0432\u043e",
            "dxDiagram-commandLayoutTopToBottom": "\u0421\u0432\u0435\u0440\u0445\u0443 \u0432\u043d\u0438\u0437",
            "dxDiagram-commandLayoutBottomToTop": "\u0421\u043d\u0438\u0437\u0443 \u0432\u0432\u0435\u0440\u0445",
            "dxDiagram-unitIn": "\u0434\u044e\u0439\u043c(\u0430)",
            "dxDiagram-unitCm": "\u0441\u043c",
            "dxDiagram-unitPx": "\u043f\u0438\u043a\u0441\u0435\u043b\u044c(\u044f)",
            "dxDiagram-dialogButtonOK": "\u041e\u041a",
            "dxDiagram-dialogButtonCancel": "\u041e\u0442\u043c\u0435\u043d\u0430",
            "dxDiagram-dialogInsertShapeImageTitle": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435",
            "dxDiagram-dialogEditShapeImageTitle": "\u0418\u0437\u043c\u0435\u043d\u0438\u0442\u044c \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435",
            "dxDiagram-dialogEditShapeImageSelectButton": "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435",
            "dxDiagram-dialogEditShapeImageLabelText": "\u0438\u043b\u0438 \u043f\u0435\u0440\u0435\u0442\u0430\u0449\u0438\u0442\u0435 \u0444\u0430\u0439\u043b \u0441\u044e\u0434\u0430",
            "dxDiagram-uiExport": "\u042d\u043a\u0441\u043f\u043e\u0440\u0442",
            "dxDiagram-uiProperties": "\u0421\u0432\u043e\u0439\u0441\u0442\u0432\u0430",
            "dxDiagram-uiSettings": "\u041d\u0430\u0441\u0442\u0440\u043e\u0439\u043a\u0438",
            "dxDiagram-uiShowToolbox": "\u041f\u0430\u043d\u0435\u043b\u044c \u0438\u043d\u0441\u0442\u0440\u0443\u043c\u0435\u043d\u0442\u043e\u0432",
            "dxDiagram-uiSearch": "\u041f\u043e\u0438\u0441\u043a",
            "dxDiagram-uiStyle": "\u0421\u0442\u0438\u043b\u044c",
            "dxDiagram-uiLayout": "\u041a\u043e\u043c\u043f\u043e\u043d\u043e\u0432\u043a\u0430",
            "dxDiagram-uiLayoutTree": "\u0414\u0440\u0435\u0432\u043e\u0432\u0438\u0434\u043d\u0430\u044f",
            "dxDiagram-uiLayoutLayered": "\u041c\u043d\u043e\u0433\u043e\u0443\u0440\u043e\u0432\u043d\u0435\u0432\u0430\u044f",
            "dxDiagram-uiDiagram": "\u0414\u0438\u0430\u0433\u0440\u0430\u043c\u043c\u0430",
            "dxDiagram-uiText": "\u0422\u0435\u043a\u0441\u0442",
            "dxDiagram-uiObject": "\u041e\u0431\u044a\u0435\u043a\u0442",
            "dxDiagram-uiConnector": "\u0421\u043e\u0435\u0434\u0438\u043d\u0438\u0442\u0435\u043b\u044c",
            "dxDiagram-uiPage": "\u0421\u0442\u0440\u0430\u043d\u0438\u0446\u0430",
            "dxDiagram-shapeText": "\u0422\u0435\u043a\u0441\u0442",
            "dxDiagram-shapeRectangle": "\u041f\u0440\u044f\u043c\u043e\u0443\u0433\u043e\u043b\u044c\u043d\u0438\u043a",
            "dxDiagram-shapeEllipse": "\u042d\u043b\u043b\u0438\u043f\u0441",
            "dxDiagram-shapeCross": "\u041a\u0440\u0435\u0441\u0442",
            "dxDiagram-shapeTriangle": "\u0422\u0440\u0435\u0443\u0433\u043e\u043b\u044c\u043d\u0438\u043a",
            "dxDiagram-shapeDiamond": "\u0420\u043e\u043c\u0431",
            "dxDiagram-shapeHeart": "\u0421\u0435\u0440\u0434\u0446\u0435",
            "dxDiagram-shapePentagon": "\u041f\u044f\u0442\u0438\u0443\u0433\u043e\u043b\u044c\u043d\u0438\u043a",
            "dxDiagram-shapeHexagon": "\u0428\u0435\u0441\u0442\u0438\u0443\u0433\u043e\u043b\u044c\u043d\u0438\u043a",
            "dxDiagram-shapeOctagon": "\u0412\u043e\u0441\u044c\u043c\u0438\u0443\u0433\u043e\u043b\u044c\u043d\u0438\u043a",
            "dxDiagram-shapeStar": "\u0417\u0432\u0435\u0437\u0434\u0430",
            "dxDiagram-shapeArrowLeft": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430 \u0432\u043b\u0435\u0432\u043e",
            "dxDiagram-shapeArrowUp": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430 \u0432\u0432\u0435\u0440\u0445",
            "dxDiagram-shapeArrowRight": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430 \u0432\u043f\u0440\u0430\u0432\u043e",
            "dxDiagram-shapeArrowDown": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430 \u0432\u043d\u0438\u0437",
            "dxDiagram-shapeArrowUpDown": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430 \u0432\u0432\u0435\u0440\u0445-\u0432\u043d\u0438\u0437",
            "dxDiagram-shapeArrowLeftRight": "\u0421\u0442\u0440\u0435\u043b\u043a\u0430 \u0432\u043b\u0435\u0432\u043e-\u0432\u043f\u0440\u0430\u0432\u043e",
            "dxDiagram-shapeProcess": "\u041f\u0440\u043e\u0446\u0435\u0441\u0441",
            "dxDiagram-shapeDecision": "\u0420\u0435\u0448\u0435\u043d\u0438\u0435",
            "dxDiagram-shapeTerminator": "\u0422\u0435\u0440\u043c\u0438\u043d\u0430\u0442\u043e\u0440",
            "dxDiagram-shapePredefinedProcess": "\u041f\u0440\u0435\u0434\u043e\u043f\u0440\u0435\u0434\u0435\u043b\u0435\u043d\u043d\u044b\u0439 \u043f\u0440\u043e\u0446\u0435\u0441\u0441",
            "dxDiagram-shapeDocument": "\u0414\u043e\u043a\u0443\u043c\u0435\u043d\u0442",
            "dxDiagram-shapeMultipleDocuments": "\u0414\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u044b",
            "dxDiagram-shapeManualInput": "\u0420\u0443\u0447\u043d\u043e\u0439 \u0432\u0432\u043e\u0434",
            "dxDiagram-shapePreparation": "\u041f\u043e\u0434\u0433\u043e\u0442\u043e\u0432\u043a\u0430",
            "dxDiagram-shapeData": "\u0414\u0430\u043d\u043d\u044b\u0435",
            "dxDiagram-shapeDatabase": "\u0411\u0430\u0437\u0430 \u0434\u0430\u043d\u043d\u044b\u0445",
            "dxDiagram-shapeHardDisk": "\u0416\u0435\u0441\u0442\u043a\u0438\u0439 \u0434\u0438\u0441\u043a",
            "dxDiagram-shapeInternalStorage": "\u0412\u043d\u0443\u0442\u0440\u0435\u043d\u043d\u044f\u044f \u043f\u0430\u043c\u044f\u0442\u044c",
            "dxDiagram-shapePaperTape": "\u0411\u0443\u043c\u0430\u0436\u043d\u0430\u044f \u043b\u0435\u043d\u0442\u0430",
            "dxDiagram-shapeManualOperation": "\u0420\u0443\u0447\u043d\u0430\u044f \u043e\u043f\u0435\u0440\u0430\u0446\u0438\u044f",
            "dxDiagram-shapeDelay": "\u0417\u0430\u0434\u0435\u0440\u0436\u043a\u0430",
            "dxDiagram-shapeStoredData": "\u0417\u0430\u043f\u043e\u043c\u0438\u043d\u0430\u0435\u043c\u044b\u0435 \u0434\u0430\u043d\u043d\u044b\u0435",
            "dxDiagram-shapeDisplay": "\u0414\u0438\u0441\u043f\u043b\u0435\u0439",
            "dxDiagram-shapeMerge": "\u0421\u043b\u0438\u044f\u043d\u0438\u0435",
            "dxDiagram-shapeConnector": "\u0421\u043e\u0435\u0434\u0438\u043d\u0438\u0442\u0435\u043b\u044c",
            "dxDiagram-shapeOr": "\u0418\u043b\u0438",
            "dxDiagram-shapeSummingJunction": "\u0421\u0443\u043c\u043c\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435",
            "dxDiagram-shapeContainerDefaultText": "\u041a\u043e\u043d\u0442\u0435\u0439\u043d\u0435\u0440",
            "dxDiagram-shapeVerticalContainer": "\u0412\u0435\u0440\u0442\u0438\u043a\u0430\u043b\u044c\u043d\u044b\u0439 \u043a\u043e\u043d\u0442\u0435\u0439\u043d\u0435\u0440",
            "dxDiagram-shapeHorizontalContainer": "\u0413\u043e\u0440\u0438\u0437\u043e\u043d\u0442\u0430\u043b\u044c\u043d\u044b\u0439 \u043a\u043e\u043d\u0442\u0435\u0439\u043d\u0435\u0440",
            "dxDiagram-shapeCardDefaultText": "\u0418\u043c\u044f \u0447\u0435\u043b\u043e\u0432\u0435\u043a\u0430",
            "dxDiagram-shapeCardWithImageOnLeft": "\u041a\u0430\u0440\u0442\u043e\u0447\u043a\u0430 \u0441 \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435\u043c \u0441\u043b\u0435\u0432\u0430",
            "dxDiagram-shapeCardWithImageOnTop": "\u041a\u0430\u0440\u0442\u043e\u0447\u043a\u0430 \u0441 \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435\u043c \u0441\u0432\u0435\u0440\u0445\u0443",
            "dxDiagram-shapeCardWithImageOnRight": "\u041a\u0430\u0440\u0442\u043e\u0447\u043a\u0430 \u0441 \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435\u043c \u0441\u043f\u0440\u0430\u0432\u0430",
            "dxGantt-dialogTitle": "\u041d\u0430\u0437\u0432\u0430\u043d\u0438\u0435",
            "dxGantt-dialogStartTitle": "\u041d\u0430\u0447\u0430\u043b\u043e",
            "dxGantt-dialogEndTitle": "\u041e\u043a\u043e\u043d\u0447\u0430\u043d\u0438\u0435",
            "dxGantt-dialogProgressTitle": "\u041f\u0440\u043e\u0433\u0440\u0435\u0441\u0441",
            "dxGantt-dialogResourcesTitle": "\u0420\u0435\u0441\u0443\u0440\u0441\u044b",
            "dxGantt-dialogResourceManagerTitle": "\u0423\u043f\u0440\u0430\u0432\u043b\u0435\u043d\u0438\u0435 \u0440\u0435\u0441\u0443\u0440\u0441\u0430\u043c\u0438",
            "dxGantt-dialogTaskDetailsTitle": "\u0414\u0435\u0442\u0430\u043b\u0438 \u0437\u0430\u0434\u0430\u0447\u0438",
            "dxGantt-dialogEditResourceListHint": "\u0420\u0435\u0434\u0430\u043a\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0441\u043f\u0438\u0441\u043e\u043a \u0440\u0435\u0441\u0443\u0440\u0441\u043e\u0432",
            "dxGantt-dialogEditNoResources": "\u0421\u043f\u0438\u0441\u043e\u043a \u0440\u0435\u0441\u0443\u0440\u0441\u043e\u0432 \u043f\u0443\u0441\u0442",
            "dxGantt-dialogButtonAdd": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c",
            "dxGantt-contextMenuNewTask": "\u041d\u043e\u0432\u0430\u044f \u0437\u0430\u0434\u0430\u0447\u0430",
            "dxGantt-contextMenuNewSubtask": "\u041d\u043e\u0432\u0430\u044f \u043f\u043e\u0434\u0437\u0430\u0434\u0430\u0447\u0430",
            "dxGantt-contextMenuDeleteTask": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c \u0437\u0430\u0434\u0430\u0447\u0443",
            "dxGantt-contextMenuDeleteDependency": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c \u0437\u0430\u0432\u0438\u0441\u0438\u043c\u043e\u0441\u0442\u044c",
            "dxGantt-dialogTaskDeleteConfirmation": "\u0423\u0434\u0430\u043b\u0435\u043d\u0438\u0435 \u0437\u0430\u0434\u0430\u0447\u0438 \u043f\u0440\u0438\u0432\u0435\u0434\u0435\u0442 \u043a \u0443\u0434\u0430\u043b\u0435\u043d\u0438\u044e \u0432\u0441\u0435\u0445 \u0435\u0451 \u0437\u0430\u0432\u0438\u0441\u0438\u043c\u043e\u0441\u0442\u0435\u0439 \u0438 \u043f\u043e\u0434\u0437\u0430\u0434\u0430\u0447. \u0412\u044b \u0443\u0432\u0435\u0440\u0435\u043d\u044b, \u0447\u0442\u043e \u0432\u044b \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c \u044d\u0442\u0443 \u0437\u0430\u0434\u0430\u0447\u0443?",
            "dxGantt-dialogDependencyDeleteConfirmation": "\u0412\u044b \u0443\u0432\u0435\u0440\u0435\u043d\u044b, \u0447\u0442\u043e \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c \u044d\u0442\u0443 \u0437\u0430\u0432\u0438\u0441\u0438\u043c\u043e\u0441\u0442\u044c \u0438\u0437 \u0437\u0430\u0434\u0430\u0447\u0438?",
            "dxGantt-dialogResourcesDeleteConfirmation": "\u0423\u0434\u0430\u043b\u0435\u043d\u0438\u0435 \u0440\u0435\u0441\u0443\u0440\u0441\u0430 \u0442\u0430\u043a\u0436\u0435 \u0443\u0434\u0430\u043b\u0438\u0442 \u0435\u0433\u043e \u0438\u0437 \u0432\u0441\u0435\u0445 \u0437\u0430\u0434\u0430\u0447, \u0432 \u043a\u043e\u0442\u043e\u0440\u044b\u0445 \u043e\u043d \u0438\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0435\u0442\u0441\u044f. \u0412\u044b \u0443\u0432\u0435\u0440\u0435\u043d\u044b, \u0447\u0442\u043e \u0445\u043e\u0442\u0438\u0442\u0435 \u0443\u0434\u0430\u043b\u0438\u0442\u044c \u044d\u0442\u0438 \u0440\u0435\u0441\u0443\u0440\u0441\u044b? \u0420\u0435\u0441\u0443\u0440\u0441\u044b: {0}",
            "dxGantt-dialogConstraintCriticalViolationMessage": "\u0417\u0430\u0434\u0430\u0447\u0430, \u043a\u043e\u0442\u043e\u0440\u0443\u044e \u0432\u044b \u043f\u0435\u0440\u0435\u0434\u0432\u0438\u0433\u0430\u0435\u0442\u0435, \u0438\u043c\u0435\u0435\u0442 \u0437\u0430\u0432\u0438\u0441\u0438\u043c\u043e\u0441\u0442\u044c \u043e\u0442 \u0434\u0440\u0443\u0433\u043e\u0439 \u0437\u0430\u0434\u0430\u0447\u0438. \u042d\u0442\u043e \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0435 \u043f\u0440\u043e\u0442\u0438\u0432\u043e\u0440\u0435\u0447\u0438\u0442 \u043f\u0440\u0430\u0432\u0438\u043b\u0430\u043c \u0432\u0430\u043b\u0438\u0434\u0430\u0446\u0438\u0438. \u041a\u0430\u043a \u0432\u044b \u0445\u043e\u0442\u0438\u0442\u0435 \u043f\u043e\u0441\u0442\u0443\u043f\u0438\u0442\u044c?",
            "dxGantt-dialogConstraintViolationMessage": "\u0417\u0430\u0434\u0430\u0447\u0430, \u043a\u043e\u0442\u043e\u0440\u0443\u044e \u0432\u044b \u043f\u0435\u0440\u0435\u0434\u0432\u0438\u0433\u0430\u0435\u0442\u0435, \u0438\u043c\u0435\u0435\u0442 \u0437\u0430\u0432\u0438\u0441\u0438\u043c\u043e\u0441\u0442\u044c \u043e\u0442 \u0434\u0440\u0443\u0433\u043e\u0439 \u0437\u0430\u0434\u0430\u0447\u0438. \u041a\u0430\u043a \u0432\u044b \u0445\u043e\u0442\u0438\u0442\u0435 \u043f\u043e\u0441\u0442\u0443\u043f\u0438\u0442\u044c?",
            "dxGantt-dialogCancelOperationMessage": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c \u043e\u043f\u0435\u0440\u0430\u0446\u0438\u044e",
            "dxGantt-dialogDeleteDependencyMessage": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c \u0437\u0430\u0434\u0430\u0447\u0443",
            "dxGantt-dialogMoveTaskAndKeepDependencyMessage": "\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c \u0437\u0430\u0432\u0438\u0441\u0438\u043c\u043e\u0441\u0442\u044c \u0438 \u043f\u0435\u0440\u0435\u0434\u0432\u0438\u043d\u0443\u0442\u044c \u0437\u0430\u0434\u0430\u0447\u0443",
            "dxGantt-undo": "\u041e\u0442\u043c\u0435\u043d\u0438\u0442\u044c",
            "dxGantt-redo": "\u041f\u043e\u0432\u0442\u043e\u0440\u0438\u0442\u044c",
            "dxGantt-expandAll": "\u0420\u0430\u0437\u0432\u0435\u0440\u043d\u0443\u0442\u044c \u0432\u0441\u0435",
            "dxGantt-collapseAll": "\u0421\u0432\u0435\u0440\u043d\u0443\u0442\u044c \u0432\u0441\u0435",
            "dxGantt-addNewTask": "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u043d\u043e\u0432\u0443\u044e \u0437\u0430\u0434\u0430\u0447\u0443",
            "dxGantt-deleteSelectedTask": "\u0423\u0434\u0430\u043b\u0438\u0442\u044c \u0432\u044b\u0434\u0435\u043b\u0435\u043d\u043d\u0443\u044e \u0437\u0430\u0434\u0430\u0447\u0443",
            "dxGantt-zoomIn": "\u0423\u0432\u0435\u043b\u0438\u0447\u0438\u0442\u044c \u043c\u0430\u0441\u0448\u0442\u0430\u0431",
            "dxGantt-zoomOut": "\u0423\u043c\u0435\u043d\u044c\u0448\u0438\u0442\u044c \u043c\u0430\u0441\u0448\u0442\u0430\u0431",
            "dxGantt-fullScreen": "\u041f\u043e\u043b\u043d\u043e\u044d\u043a\u0440\u0430\u043d\u043d\u044b\u0439 \u0440\u0435\u0436\u0438\u043c"
        }
    })
});
