/**
 * Cesium - https://github.com/CesiumGS/cesium
 *
 * Copyright 2011-2020 Cesium Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Columbus View (Pat. Pend.)
 *
 * Portions licensed separately.
 * See https://github.com/CesiumGS/cesium/blob/master/LICENSE.md for full licensing details.
 */

define(['./when-b43ff45e', './RuntimeError-bf10f3d5', './WebGLConstants-56de22c0', './createTaskProcessorWorker'], function (when, RuntimeError, WebGLConstants, createTaskProcessorWorker) {
    'use strict';

    /**
     * Describes a compressed texture and contains a compressed texture buffer.
     * @alias CompressedTextureBuffer
     * @constructor
     *
     * @param {PixelFormat} internalFormat The pixel format of the compressed texture.
     * @param {Number} width The width of the texture.
     * @param {Number} height The height of the texture.
     * @param {Uint8Array} buffer The compressed texture buffer.
     */
    function CompressedTextureBuffer(internalFormat, width, height, buffer) {
        this._format = internalFormat;
        this._width = width;
        this._height = height;
        this._buffer = buffer;
    }

    Object.defineProperties(CompressedTextureBuffer.prototype, {
        /**
         * The format of the compressed texture.
         * @type PixelFormat
         * @readonly
         * @memberof CompressedTextureBuffer.prototype
         */
        internalFormat: {
            get: function () {
                return this._format;
            },
        },
        /**
         * The width of the texture.
         * @type Number
         * @readonly
         * @memberof CompressedTextureBuffer.prototype
         */
        width: {
            get: function () {
                return this._width;
            },
        },
        /**
         * The height of the texture.
         * @type Number
         * @readonly
         * @memberof CompressedTextureBuffer.prototype
         */
        height: {
            get: function () {
                return this._height;
            },
        },
        /**
         * The compressed texture buffer.
         * @type Uint8Array
         * @readonly
         * @memberof CompressedTextureBuffer.prototype
         */
        bufferView: {
            get: function () {
                return this._buffer;
            },
        },
    });

    /**
     * Creates a shallow clone of a compressed texture buffer.
     *
     * @param {CompressedTextureBuffer} object The compressed texture buffer to be cloned.
     * @return {CompressedTextureBuffer} A shallow clone of the compressed texture buffer.
     */
    CompressedTextureBuffer.clone = function (object) {
        if (!when.defined(object)) {
            return undefined;
        }

        return new CompressedTextureBuffer(
            object._format,
            object._width,
            object._height,
            object._buffer
        );
    };

    /**
     * Creates a shallow clone of this compressed texture buffer.
     *
     * @return {CompressedTextureBuffer} A shallow clone of the compressed texture buffer.
     */
    CompressedTextureBuffer.prototype.clone = function () {
        return CompressedTextureBuffer.clone(this);
    };

    /**
     * @private
     */
    var PixelDatatype = {
        UNSIGNED_BYTE: WebGLConstants.WebGLConstants.UNSIGNED_BYTE,
        UNSIGNED_SHORT: WebGLConstants.WebGLConstants.UNSIGNED_SHORT,
        UNSIGNED_INT: WebGLConstants.WebGLConstants.UNSIGNED_INT,
        FLOAT: WebGLConstants.WebGLConstants.FLOAT,
        HALF_FLOAT: WebGLConstants.WebGLConstants.HALF_FLOAT_OES,
        UNSIGNED_INT_24_8: WebGLConstants.WebGLConstants.UNSIGNED_INT_24_8,
        UNSIGNED_SHORT_4_4_4_4: WebGLConstants.WebGLConstants.UNSIGNED_SHORT_4_4_4_4,
        UNSIGNED_SHORT_5_5_5_1: WebGLConstants.WebGLConstants.UNSIGNED_SHORT_5_5_5_1,
        UNSIGNED_SHORT_5_6_5: WebGLConstants.WebGLConstants.UNSIGNED_SHORT_5_6_5,

        isPacked: function (pixelDatatype) {
            return (
                pixelDatatype === PixelDatatype.UNSIGNED_INT_24_8 ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT_4_4_4_4 ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT_5_5_5_1 ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT_5_6_5
            );
        },

        sizeInBytes: function (pixelDatatype) {
            switch (pixelDatatype) {
                case PixelDatatype.UNSIGNED_BYTE:
                    return 1;
                case PixelDatatype.UNSIGNED_SHORT:
                case PixelDatatype.UNSIGNED_SHORT_4_4_4_4:
                case PixelDatatype.UNSIGNED_SHORT_5_5_5_1:
                case PixelDatatype.UNSIGNED_SHORT_5_6_5:
                case PixelDatatype.HALF_FLOAT:
                    return 2;
                case PixelDatatype.UNSIGNED_INT:
                case PixelDatatype.FLOAT:
                case PixelDatatype.UNSIGNED_INT_24_8:
                    return 4;
            }
        },

        validate: function (pixelDatatype) {
            return (
                pixelDatatype === PixelDatatype.UNSIGNED_BYTE ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT ||
                pixelDatatype === PixelDatatype.UNSIGNED_INT ||
                pixelDatatype === PixelDatatype.FLOAT ||
                pixelDatatype === PixelDatatype.HALF_FLOAT ||
                pixelDatatype === PixelDatatype.UNSIGNED_INT_24_8 ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT_4_4_4_4 ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT_5_5_5_1 ||
                pixelDatatype === PixelDatatype.UNSIGNED_SHORT_5_6_5
            );
        },
    };
    var PixelDatatype$1 = Object.freeze(PixelDatatype);

    /**
     * The format of a pixel, i.e., the number of components it has and what they represent.
     *
     * @exports PixelFormat
     */
    var PixelFormat = {
        /**
         * A pixel format containing a depth value.
         *
         * @type {Number}
         * @constant
         */
        DEPTH_COMPONENT: WebGLConstants.WebGLConstants.DEPTH_COMPONENT,

        /**
         * A pixel format containing a depth and stencil value, most often used with {@link PixelDatatype.UNSIGNED_INT_24_8}.
         *
         * @type {Number}
         * @constant
         */
        DEPTH_STENCIL: WebGLConstants.WebGLConstants.DEPTH_STENCIL,

        /**
         * A pixel format containing an alpha channel.
         *
         * @type {Number}
         * @constant
         */
        ALPHA: WebGLConstants.WebGLConstants.ALPHA,

        /**
         * A pixel format containing red, green, and blue channels.
         *
         * @type {Number}
         * @constant
         */
        RGB: WebGLConstants.WebGLConstants.RGB,

        /**
         * A pixel format containing red, green, blue, and alpha channels.
         *
         * @type {Number}
         * @constant
         */
        RGBA: WebGLConstants.WebGLConstants.RGBA,

        /**
         * A pixel format containing a luminance (intensity) channel.
         *
         * @type {Number}
         * @constant
         */
        LUMINANCE: WebGLConstants.WebGLConstants.LUMINANCE,

        /**
         * A pixel format containing luminance (intensity) and alpha channels.
         *
         * @type {Number}
         * @constant
         */
        LUMINANCE_ALPHA: WebGLConstants.WebGLConstants.LUMINANCE_ALPHA,

        /**
         * A pixel format containing red, green, and blue channels that is DXT1 compressed.
         *
         * @type {Number}
         * @constant
         */
        RGB_DXT1: WebGLConstants.WebGLConstants.COMPRESSED_RGB_S3TC_DXT1_EXT,

        /**
         * A pixel format containing red, green, blue, and alpha channels that is DXT1 compressed.
         *
         * @type {Number}
         * @constant
         */
        RGBA_DXT1: WebGLConstants.WebGLConstants.COMPRESSED_RGBA_S3TC_DXT1_EXT,

        /**
         * A pixel format containing red, green, blue, and alpha channels that is DXT3 compressed.
         *
         * @type {Number}
         * @constant
         */
        RGBA_DXT3: WebGLConstants.WebGLConstants.COMPRESSED_RGBA_S3TC_DXT3_EXT,

        /**
         * A pixel format containing red, green, blue, and alpha channels that is DXT5 compressed.
         *
         * @type {Number}
         * @constant
         */
        RGBA_DXT5: WebGLConstants.WebGLConstants.COMPRESSED_RGBA_S3TC_DXT5_EXT,

        /**
         * A pixel format containing red, green, and blue channels that is PVR 4bpp compressed.
         *
         * @type {Number}
         * @constant
         */
        RGB_PVRTC_4BPPV1: WebGLConstants.WebGLConstants.COMPRESSED_RGB_PVRTC_4BPPV1_IMG,

        /**
         * A pixel format containing red, green, and blue channels that is PVR 2bpp compressed.
         *
         * @type {Number}
         * @constant
         */
        RGB_PVRTC_2BPPV1: WebGLConstants.WebGLConstants.COMPRESSED_RGB_PVRTC_2BPPV1_IMG,

        /**
         * A pixel format containing red, green, blue, and alpha channels that is PVR 4bpp compressed.
         *
         * @type {Number}
         * @constant
         */
        RGBA_PVRTC_4BPPV1: WebGLConstants.WebGLConstants.COMPRESSED_RGBA_PVRTC_4BPPV1_IMG,

        /**
         * A pixel format containing red, green, blue, and alpha channels that is PVR 2bpp compressed.
         *
         * @type {Number}
         * @constant
         */
        RGBA_PVRTC_2BPPV1: WebGLConstants.WebGLConstants.COMPRESSED_RGBA_PVRTC_2BPPV1_IMG,

        /**
         * A pixel format containing red, green, and blue channels that is ETC1 compressed.
         *
         * @type {Number}
         * @constant
         */
        RGB_ETC1: WebGLConstants.WebGLConstants.COMPRESSED_RGB_ETC1_WEBGL,

        /**
         * @private
         */
        componentsLength: function (pixelFormat) {
            switch (pixelFormat) {
                case PixelFormat.RGB:
                    return 3;
                case PixelFormat.RGBA:
                    return 4;
                case PixelFormat.LUMINANCE_ALPHA:
                    return 2;
                case PixelFormat.ALPHA:
                case PixelFormat.LUMINANCE:
                    return 1;
                default:
                    return 1;
            }
        },

        /**
         * @private
         */
        validate: function (pixelFormat) {
            return (
                pixelFormat === PixelFormat.DEPTH_COMPONENT ||
                pixelFormat === PixelFormat.DEPTH_STENCIL ||
                pixelFormat === PixelFormat.ALPHA ||
                pixelFormat === PixelFormat.RGB ||
                pixelFormat === PixelFormat.RGBA ||
                pixelFormat === PixelFormat.LUMINANCE ||
                pixelFormat === PixelFormat.LUMINANCE_ALPHA ||
                pixelFormat === PixelFormat.RGB_DXT1 ||
                pixelFormat === PixelFormat.RGBA_DXT1 ||
                pixelFormat === PixelFormat.RGBA_DXT3 ||
                pixelFormat === PixelFormat.RGBA_DXT5 ||
                pixelFormat === PixelFormat.RGB_PVRTC_4BPPV1 ||
                pixelFormat === PixelFormat.RGB_PVRTC_2BPPV1 ||
                pixelFormat === PixelFormat.RGBA_PVRTC_4BPPV1 ||
                pixelFormat === PixelFormat.RGBA_PVRTC_2BPPV1 ||
                pixelFormat === PixelFormat.RGB_ETC1
            );
        },

        /**
         * @private
         */
        isColorFormat: function (pixelFormat) {
            return (
                pixelFormat === PixelFormat.ALPHA ||
                pixelFormat === PixelFormat.RGB ||
                pixelFormat === PixelFormat.RGBA ||
                pixelFormat === PixelFormat.LUMINANCE ||
                pixelFormat === PixelFormat.LUMINANCE_ALPHA
            );
        },

        /**
         * @private
         */
        isDepthFormat: function (pixelFormat) {
            return (
                pixelFormat === PixelFormat.DEPTH_COMPONENT ||
                pixelFormat === PixelFormat.DEPTH_STENCIL
            );
        },

        /**
         * @private
         */
        isCompressedFormat: function (pixelFormat) {
            return (
                pixelFormat === PixelFormat.RGB_DXT1 ||
                pixelFormat === PixelFormat.RGBA_DXT1 ||
                pixelFormat === PixelFormat.RGBA_DXT3 ||
                pixelFormat === PixelFormat.RGBA_DXT5 ||
                pixelFormat === PixelFormat.RGB_PVRTC_4BPPV1 ||
                pixelFormat === PixelFormat.RGB_PVRTC_2BPPV1 ||
                pixelFormat === PixelFormat.RGBA_PVRTC_4BPPV1 ||
                pixelFormat === PixelFormat.RGBA_PVRTC_2BPPV1 ||
                pixelFormat === PixelFormat.RGB_ETC1
            );
        },

        /**
         * @private
         */
        isDXTFormat: function (pixelFormat) {
            return (
                pixelFormat === PixelFormat.RGB_DXT1 ||
                pixelFormat === PixelFormat.RGBA_DXT1 ||
                pixelFormat === PixelFormat.RGBA_DXT3 ||
                pixelFormat === PixelFormat.RGBA_DXT5
            );
        },

        /**
         * @private
         */
        isPVRTCFormat: function (pixelFormat) {
            return (
                pixelFormat === PixelFormat.RGB_PVRTC_4BPPV1 ||
                pixelFormat === PixelFormat.RGB_PVRTC_2BPPV1 ||
                pixelFormat === PixelFormat.RGBA_PVRTC_4BPPV1 ||
                pixelFormat === PixelFormat.RGBA_PVRTC_2BPPV1
            );
        },

        /**
         * @private
         */
        isETC1Format: function (pixelFormat) {
            return pixelFormat === PixelFormat.RGB_ETC1;
        },

        /**
         * @private
         */
        compressedTextureSizeInBytes: function (pixelFormat, width, height) {
            switch (pixelFormat) {
                case PixelFormat.RGB_DXT1:
                case PixelFormat.RGBA_DXT1:
                case PixelFormat.RGB_ETC1:
                    return Math.floor((width + 3) / 4) * Math.floor((height + 3) / 4) * 8;

                case PixelFormat.RGBA_DXT3:
                case PixelFormat.RGBA_DXT5:
                    return Math.floor((width + 3) / 4) * Math.floor((height + 3) / 4) * 16;

                case PixelFormat.RGB_PVRTC_4BPPV1:
                case PixelFormat.RGBA_PVRTC_4BPPV1:
                    return Math.floor(
                        (Math.max(width, 8) * Math.max(height, 8) * 4 + 7) / 8
                    );

                case PixelFormat.RGB_PVRTC_2BPPV1:
                case PixelFormat.RGBA_PVRTC_2BPPV1:
                    return Math.floor(
                        (Math.max(width, 16) * Math.max(height, 8) * 2 + 7) / 8
                    );

                default:
                    return 0;
            }
        },

        /**
         * @private
         */
        textureSizeInBytes: function (pixelFormat, pixelDatatype, width, height) {
            var componentsLength = PixelFormat.componentsLength(pixelFormat);
            if (PixelDatatype$1.isPacked(pixelDatatype)) {
                componentsLength = 1;
            }
            return (
                componentsLength *
                PixelDatatype$1.sizeInBytes(pixelDatatype) *
                width *
                height
            );
        },

        /**
         * @private
         */
        alignmentInBytes: function (pixelFormat, pixelDatatype, width) {
            var mod =
                PixelFormat.textureSizeInBytes(pixelFormat, pixelDatatype, width, 1) % 4;
            return mod === 0 ? 4 : mod === 2 ? 2 : 1;
        },

        /**
         * @private
         */
        createTypedArray: function (pixelFormat, pixelDatatype, width, height) {
            var constructor;
            var sizeInBytes = PixelDatatype$1.sizeInBytes(pixelDatatype);
            if (sizeInBytes === Uint8Array.BYTES_PER_ELEMENT) {
                constructor = Uint8Array;
            } else if (sizeInBytes === Uint16Array.BYTES_PER_ELEMENT) {
                constructor = Uint16Array;
            } else if (
                sizeInBytes === Float32Array.BYTES_PER_ELEMENT &&
                pixelDatatype === PixelDatatype$1.FLOAT
            ) {
                constructor = Float32Array;
            } else {
                constructor = Uint32Array;
            }

            var size = PixelFormat.componentsLength(pixelFormat) * width * height;
            return new constructor(size);
        },

        /**
         * @private
         */
        flipY: function (bufferView, pixelFormat, pixelDatatype, width, height) {
            if (height === 1) {
                return bufferView;
            }
            var flipped = PixelFormat.createTypedArray(
                pixelFormat,
                pixelDatatype,
                width,
                height
            );
            var numberOfComponents = PixelFormat.componentsLength(pixelFormat);
            var textureWidth = width * numberOfComponents;
            for (var i = 0; i < height; ++i) {
                var row = i * height * numberOfComponents;
                var flippedRow = (height - i - 1) * height * numberOfComponents;
                for (var j = 0; j < textureWidth; ++j) {
                    flipped[flippedRow + j] = bufferView[row + j];
                }
            }
            return flipped;
        },
    };
    var PixelFormat$1 = Object.freeze(PixelFormat);

    /**
     * @licence
     * crunch/crnlib uses the ZLIB license:
     * http://opensource.org/licenses/Zlib
     *
     * Copyright (c) 2010-2016 Richard Geldreich, Jr. and Binomial LLC
     *
     * This software is provided 'as-is', without any express or implied
     * warranty.  In no event will the authors be held liable for any damages
     * arising from the use of this software.
     *
     * Permission is granted to anyone to use this software for any purpose,
     * including commercial applications, and to alter it and redistribute it
     * freely, subject to the following restrictions:
     *
     * 1. The origin of this software must not be misrepresented; you must not
     * claim that you wrote the original software. If you use this software
     * in a product, an acknowledgment in the product documentation would be
     * appreciated but is not required.
     *
     * 2. Altered source versions must be plainly marked as such, and must not be
     * misrepresented as being the original software.
     *
     * 3. This notice may not be removed or altered from any source distribution.
     */

        // The C++ code was compiled to Javascript with Emcripten.
        // For instructions, see: https://github.com/BinomialLLC/crunch

    var Module;
    if (!Module) Module = (typeof Module !== "undefined" ? Module : null) || {};
    var moduleOverrides = {};
    for (var key in Module) {
        if (Module.hasOwnProperty(key)) {
            moduleOverrides[key] = Module[key];
        }
    }
    var ENVIRONMENT_IS_WEB = false;
    var ENVIRONMENT_IS_WORKER = false;
    var ENVIRONMENT_IS_NODE = false;
    var ENVIRONMENT_IS_SHELL = false;
    if (Module["ENVIRONMENT"]) {
        if (Module["ENVIRONMENT"] === "WEB") {
            ENVIRONMENT_IS_WEB = true;
        } else if (Module["ENVIRONMENT"] === "WORKER") {
            ENVIRONMENT_IS_WORKER = true;
        } else if (Module["ENVIRONMENT"] === "NODE") {
            ENVIRONMENT_IS_NODE = true;
        } else if (Module["ENVIRONMENT"] === "SHELL") {
            ENVIRONMENT_IS_SHELL = true;
        } else {
            throw new Error("The provided Module['ENVIRONMENT'] value is not valid. It must be one of: WEB|WORKER|NODE|SHELL.")
        }
    } else {
        ENVIRONMENT_IS_WEB = typeof window === "object";
        ENVIRONMENT_IS_WORKER = typeof importScripts === "function";
        ENVIRONMENT_IS_NODE = typeof process === "object" && typeof require === "function" && !ENVIRONMENT_IS_WEB && !ENVIRONMENT_IS_WORKER;
        ENVIRONMENT_IS_SHELL = !ENVIRONMENT_IS_WEB && !ENVIRONMENT_IS_NODE && !ENVIRONMENT_IS_WORKER;
    }
    if (ENVIRONMENT_IS_NODE) {
        if (!Module["print"]) Module["print"] = console.log;
        if (!Module["printErr"]) Module["printErr"] = console.warn;
        var nodeFS;
        var nodePath;
        Module["read"] = function shell_read(filename, binary) {
            if (!nodeFS) nodeFS = require("fs");
            if (!nodePath) nodePath = require("path");
            filename = nodePath["normalize"](filename);
            var ret = nodeFS["readFileSync"](filename);
            return binary ? ret : ret.toString()
        };
        Module["readBinary"] = function readBinary(filename) {
            var ret = Module["read"](filename, true);
            if (!ret.buffer) {
                ret = new Uint8Array(ret);
            }
            assert(ret.buffer);
            return ret
        };
        Module["load"] = function load(f) {
            globalEval(read(f));
        };
        if (!Module["thisProgram"]) {
            if (process["argv"].length > 1) {
                Module["thisProgram"] = process["argv"][1].replace(/\\/g, "/");
            } else {
                Module["thisProgram"] = "unknown-program";
            }
        }
        Module["arguments"] = process["argv"].slice(2);
        if (typeof module !== "undefined") {
            module["exports"] = Module;
        }
        process["on"]("uncaughtException", (function (ex) {
            if (!(ex instanceof ExitStatus)) {
                throw ex
            }
        }));
        Module["inspect"] = (function () {
            return "[Emscripten Module object]"
        });
    } else if (ENVIRONMENT_IS_SHELL) {
        if (!Module["print"]) Module["print"] = print;
        if (typeof printErr != "undefined") Module["printErr"] = printErr;
        if (typeof read != "undefined") {
            Module["read"] = read;
        } else {
            Module["read"] = function shell_read() {
                throw "no read() available"
            };
        }
        Module["readBinary"] = function readBinary(f) {
            if (typeof readbuffer === "function") {
                return new Uint8Array(readbuffer(f))
            }
            var data = read(f, "binary");
            assert(typeof data === "object");
            return data
        };
        if (typeof scriptArgs != "undefined") {
            Module["arguments"] = scriptArgs;
        } else if (typeof arguments != "undefined") {
            Module["arguments"] = arguments;
        }
        if (typeof quit === "function") {
            Module["quit"] = (function (status, toThrow) {
                quit(status);
            });
        }
    } else if (ENVIRONMENT_IS_WEB || ENVIRONMENT_IS_WORKER) {
        Module["read"] = function shell_read(url) {
            var xhr = new XMLHttpRequest;
            xhr.open("GET", url, false);
            xhr.send(null);
            return xhr.responseText
        };
        if (ENVIRONMENT_IS_WORKER) {
            Module["readBinary"] = function readBinary(url) {
                var xhr = new XMLHttpRequest;
                xhr.open("GET", url, false);
                xhr.responseType = "arraybuffer";
                xhr.send(null);
                return new Uint8Array(xhr.response)
            };
        }
        Module["readAsync"] = function readAsync(url, onload, onerror) {
            var xhr = new XMLHttpRequest;
            xhr.open("GET", url, true);
            xhr.responseType = "arraybuffer";
            xhr.onload = function xhr_onload() {
                if (xhr.status == 200 || xhr.status == 0 && xhr.response) {
                    onload(xhr.response);
                } else {
                    onerror();
                }
            };
            xhr.onerror = onerror;
            xhr.send(null);
        };
        if (typeof arguments != "undefined") {
            Module["arguments"] = arguments;
        }
        if (typeof console !== "undefined") {
            if (!Module["print"]) Module["print"] = function shell_print(x) {
                console.log(x);
            };
            if (!Module["printErr"]) Module["printErr"] = function shell_printErr(x) {
                console.warn(x);
            };
        } else {
            var TRY_USE_DUMP = false;
            if (!Module["print"]) Module["print"] = TRY_USE_DUMP && typeof dump !== "undefined" ? (function (x) {
                dump(x);
            }) : (function (x) {
            });
        }
        if (ENVIRONMENT_IS_WORKER) {
            Module["load"] = importScripts;
        }
        if (typeof Module["setWindowTitle"] === "undefined") {
            Module["setWindowTitle"] = (function (title) {
                document.title = title;
            });
        }
    } else {
        throw "Unknown runtime environment. Where are we?"
    }

    function globalEval(x) {
        eval.call(null, x);
    }

    if (!Module["load"] && Module["read"]) {
        Module["load"] = function load(f) {
            globalEval(Module["read"](f));
        };
    }
    if (!Module["print"]) {
        Module["print"] = (function () {
        });
    }
    if (!Module["printErr"]) {
        Module["printErr"] = Module["print"];
    }
    if (!Module["arguments"]) {
        Module["arguments"] = [];
    }
    if (!Module["thisProgram"]) {
        Module["thisProgram"] = "./this.program";
    }
    if (!Module["quit"]) {
        Module["quit"] = (function (status, toThrow) {
            throw toThrow
        });
    }
    Module.print = Module["print"];
    Module.printErr = Module["printErr"];
    Module["preRun"] = [];
    Module["postRun"] = [];
    for (var key in moduleOverrides) {
        if (moduleOverrides.hasOwnProperty(key)) {
            Module[key] = moduleOverrides[key];
        }
    }
    moduleOverrides = undefined;
    var Runtime = {
        setTempRet0: (function (value) {
            tempRet0 = value;
            return value
        }), getTempRet0: (function () {
            return tempRet0
        }), stackSave: (function () {
            return STACKTOP
        }), stackRestore: (function (stackTop) {
            STACKTOP = stackTop;
        }), getNativeTypeSize: (function (type) {
            switch (type) {
                case"i1":
                case"i8":
                    return 1;
                case"i16":
                    return 2;
                case"i32":
                    return 4;
                case"i64":
                    return 8;
                case"float":
                    return 4;
                case"double":
                    return 8;
                default: {
                    if (type[type.length - 1] === "*") {
                        return Runtime.QUANTUM_SIZE
                    } else if (type[0] === "i") {
                        var bits = parseInt(type.substr(1));
                        assert(bits % 8 === 0);
                        return bits / 8
                    } else {
                        return 0
                    }
                }
            }
        }), getNativeFieldSize: (function (type) {
            return Math.max(Runtime.getNativeTypeSize(type), Runtime.QUANTUM_SIZE)
        }), STACK_ALIGN: 16, prepVararg: (function (ptr, type) {
            if (type === "double" || type === "i64") {
                if (ptr & 7) {
                    assert((ptr & 7) === 4);
                    ptr += 4;
                }
            } else {
                assert((ptr & 3) === 0);
            }
            return ptr
        }), getAlignSize: (function (type, size, vararg) {
            if (!vararg && (type == "i64" || type == "double")) return 8;
            if (!type) return Math.min(size, 8);
            return Math.min(size || (type ? Runtime.getNativeFieldSize(type) : 0), Runtime.QUANTUM_SIZE)
        }), dynCall: (function (sig, ptr, args) {
            if (args && args.length) {
                return Module["dynCall_" + sig].apply(null, [ptr].concat(args))
            } else {
                return Module["dynCall_" + sig].call(null, ptr)
            }
        }), functionPointers: [], addFunction: (function (func) {
            for (var i = 0; i < Runtime.functionPointers.length; i++) {
                if (!Runtime.functionPointers[i]) {
                    Runtime.functionPointers[i] = func;
                    return 2 * (1 + i)
                }
            }
            throw "Finished up all reserved function pointers. Use a higher value for RESERVED_FUNCTION_POINTERS."
        }), removeFunction: (function (index) {
            Runtime.functionPointers[(index - 2) / 2] = null;
        }), warnOnce: (function (text) {
            if (!Runtime.warnOnce.shown) Runtime.warnOnce.shown = {};
            if (!Runtime.warnOnce.shown[text]) {
                Runtime.warnOnce.shown[text] = 1;
                Module.printErr(text);
            }
        }), funcWrappers: {}, getFuncWrapper: (function (func, sig) {
            assert(sig);
            if (!Runtime.funcWrappers[sig]) {
                Runtime.funcWrappers[sig] = {};
            }
            var sigCache = Runtime.funcWrappers[sig];
            if (!sigCache[func]) {
                if (sig.length === 1) {
                    sigCache[func] = function dynCall_wrapper() {
                        return Runtime.dynCall(sig, func)
                    };
                } else if (sig.length === 2) {
                    sigCache[func] = function dynCall_wrapper(arg) {
                        return Runtime.dynCall(sig, func, [arg])
                    };
                } else {
                    sigCache[func] = function dynCall_wrapper() {
                        return Runtime.dynCall(sig, func, Array.prototype.slice.call(arguments))
                    };
                }
            }
            return sigCache[func]
        }), getCompilerSetting: (function (name) {
            throw "You must build with -s RETAIN_COMPILER_SETTINGS=1 for Runtime.getCompilerSetting or emscripten_get_compiler_setting to work"
        }), stackAlloc: (function (size) {
            var ret = STACKTOP;
            STACKTOP = STACKTOP + size | 0;
            STACKTOP = STACKTOP + 15 & -16;
            return ret
        }), staticAlloc: (function (size) {
            var ret = STATICTOP;
            STATICTOP = STATICTOP + size | 0;
            STATICTOP = STATICTOP + 15 & -16;
            return ret
        }), dynamicAlloc: (function (size) {
            var ret = HEAP32[DYNAMICTOP_PTR >> 2];
            var end = (ret + size + 15 | 0) & -16;
            HEAP32[DYNAMICTOP_PTR >> 2] = end;
            if (end >= TOTAL_MEMORY) {
                var success = enlargeMemory();
                if (!success) {
                    HEAP32[DYNAMICTOP_PTR >> 2] = ret;
                    return 0
                }
            }
            return ret
        }), alignMemory: (function (size, quantum) {
            var ret = size = Math.ceil(size / (quantum ? quantum : 16)) * (quantum ? quantum : 16);
            return ret
        }), makeBigInt: (function (low, high, unsigned) {
            var ret = unsigned ? +(low >>> 0) + +(high >>> 0) * +4294967296 : +(low >>> 0) + +(high | 0) * +4294967296;
            return ret
        }), GLOBAL_BASE: 8, QUANTUM_SIZE: 4, __dummy__: 0
    };
    Module["Runtime"] = Runtime;
    var ABORT = 0;

    function assert(condition, text) {
        if (!condition) {
            abort("Assertion failed: " + text);
        }
    }

    function getCFunc(ident) {
        var func = Module["_" + ident];
        if (!func) {
            try {
                func = eval("_" + ident);
            } catch (e) {
            }
        }
        assert(func, "Cannot call unknown function " + ident + " (perhaps LLVM optimizations or closure removed it?)");
        return func
    }

    var cwrap, ccall;
    ((function () {
        var JSfuncs = {
            "stackSave": (function () {
                Runtime.stackSave();
            }), "stackRestore": (function () {
                Runtime.stackRestore();
            }), "arrayToC": (function (arr) {
                var ret = Runtime.stackAlloc(arr.length);
                writeArrayToMemory(arr, ret);
                return ret
            }), "stringToC": (function (str) {
                var ret = 0;
                if (str !== null && str !== undefined && str !== 0) {
                    var len = (str.length << 2) + 1;
                    ret = Runtime.stackAlloc(len);
                    stringToUTF8(str, ret, len);
                }
                return ret
            })
        };
        var toC = {"string": JSfuncs["stringToC"], "array": JSfuncs["arrayToC"]};
        ccall = function ccallFunc(ident, returnType, argTypes, args, opts) {
            var func = getCFunc(ident);
            var cArgs = [];
            var stack = 0;
            if (args) {
                for (var i = 0; i < args.length; i++) {
                    var converter = toC[argTypes[i]];
                    if (converter) {
                        if (stack === 0) stack = Runtime.stackSave();
                        cArgs[i] = converter(args[i]);
                    } else {
                        cArgs[i] = args[i];
                    }
                }
            }
            var ret = func.apply(null, cArgs);
            if (returnType === "string") ret = Pointer_stringify(ret);
            if (stack !== 0) {
                if (opts && opts.async) {
                    EmterpreterAsync.asyncFinalizers.push((function () {
                        Runtime.stackRestore(stack);
                    }));
                    return
                }
                Runtime.stackRestore(stack);
            }
            return ret
        };
        var sourceRegex = /^function\s*[a-zA-Z$_0-9]*\s*\(([^)]*)\)\s*{\s*([^*]*?)[\s;]*(?:return\s*(.*?)[;\s]*)?}$/;

        function parseJSFunc(jsfunc) {
            var parsed = jsfunc.toString().match(sourceRegex).slice(1);
            return {arguments: parsed[0], body: parsed[1], returnValue: parsed[2]}
        }

        var JSsource = null;

        function ensureJSsource() {
            if (!JSsource) {
                JSsource = {};
                for (var fun in JSfuncs) {
                    if (JSfuncs.hasOwnProperty(fun)) {
                        JSsource[fun] = parseJSFunc(JSfuncs[fun]);
                    }
                }
            }
        }

        cwrap = function cwrap(ident, returnType, argTypes) {
            argTypes = argTypes || [];
            var cfunc = getCFunc(ident);
            var numericArgs = argTypes.every((function (type) {
                return type === "number"
            }));
            var numericRet = returnType !== "string";
            if (numericRet && numericArgs) {
                return cfunc
            }
            var argNames = argTypes.map((function (x, i) {
                return "$" + i
            }));
            var funcstr = "(function(" + argNames.join(",") + ") {";
            var nargs = argTypes.length;
            if (!numericArgs) {
                ensureJSsource();
                funcstr += "var stack = " + JSsource["stackSave"].body + ";";
                for (var i = 0; i < nargs; i++) {
                    var arg = argNames[i], type = argTypes[i];
                    if (type === "number") continue;
                    var convertCode = JSsource[type + "ToC"];
                    funcstr += "var " + convertCode.arguments + " = " + arg + ";";
                    funcstr += convertCode.body + ";";
                    funcstr += arg + "=(" + convertCode.returnValue + ");";
                }
            }
            var cfuncname = parseJSFunc((function () {
                return cfunc
            })).returnValue;
            funcstr += "var ret = " + cfuncname + "(" + argNames.join(",") + ");";
            if (!numericRet) {
                var strgfy = parseJSFunc((function () {
                    return Pointer_stringify
                })).returnValue;
                funcstr += "ret = " + strgfy + "(ret);";
            }
            if (!numericArgs) {
                ensureJSsource();
                funcstr += JSsource["stackRestore"].body.replace("()", "(stack)") + ";";
            }
            funcstr += "return ret})";
            return eval(funcstr)
        };
    }))();
    Module["ccall"] = ccall;
    Module["cwrap"] = cwrap;

    function setValue(ptr, value, type, noSafe) {
        type = type || "i8";
        if (type.charAt(type.length - 1) === "*") type = "i32";
        switch (type) {
            case"i1":
                HEAP8[ptr >> 0] = value;
                break;
            case"i8":
                HEAP8[ptr >> 0] = value;
                break;
            case"i16":
                HEAP16[ptr >> 1] = value;
                break;
            case"i32":
                HEAP32[ptr >> 2] = value;
                break;
            case"i64":
                tempI64 = [value >>> 0, (tempDouble = value, +Math_abs(tempDouble) >= +1 ? tempDouble > +0 ? (Math_min(+Math_floor(tempDouble / +4294967296), +4294967295) | 0) >>> 0 : ~~+Math_ceil((tempDouble - +(~~tempDouble >>> 0)) / +4294967296) >>> 0 : 0)], HEAP32[ptr >> 2] = tempI64[0], HEAP32[ptr + 4 >> 2] = tempI64[1];
                break;
            case"float":
                HEAPF32[ptr >> 2] = value;
                break;
            case"double":
                HEAPF64[ptr >> 3] = value;
                break;
            default:
                abort("invalid type for setValue: " + type);
        }
    }

    Module["setValue"] = setValue;

    function getValue(ptr, type, noSafe) {
        type = type || "i8";
        if (type.charAt(type.length - 1) === "*") type = "i32";
        switch (type) {
            case"i1":
                return HEAP8[ptr >> 0];
            case"i8":
                return HEAP8[ptr >> 0];
            case"i16":
                return HEAP16[ptr >> 1];
            case"i32":
                return HEAP32[ptr >> 2];
            case"i64":
                return HEAP32[ptr >> 2];
            case"float":
                return HEAPF32[ptr >> 2];
            case"double":
                return HEAPF64[ptr >> 3];
            default:
                abort("invalid type for setValue: " + type);
        }
        return null
    }

    Module["getValue"] = getValue;
    var ALLOC_NORMAL = 0;
    var ALLOC_STACK = 1;
    var ALLOC_STATIC = 2;
    var ALLOC_DYNAMIC = 3;
    var ALLOC_NONE = 4;
    Module["ALLOC_NORMAL"] = ALLOC_NORMAL;
    Module["ALLOC_STACK"] = ALLOC_STACK;
    Module["ALLOC_STATIC"] = ALLOC_STATIC;
    Module["ALLOC_DYNAMIC"] = ALLOC_DYNAMIC;
    Module["ALLOC_NONE"] = ALLOC_NONE;

    function allocate(slab, types, allocator, ptr) {
        var zeroinit, size;
        if (typeof slab === "number") {
            zeroinit = true;
            size = slab;
        } else {
            zeroinit = false;
            size = slab.length;
        }
        var singleType = typeof types === "string" ? types : null;
        var ret;
        if (allocator == ALLOC_NONE) {
            ret = ptr;
        } else {
            ret = [typeof _malloc === "function" ? _malloc : Runtime.staticAlloc, Runtime.stackAlloc, Runtime.staticAlloc, Runtime.dynamicAlloc][allocator === undefined ? ALLOC_STATIC : allocator](Math.max(size, singleType ? 1 : types.length));
        }
        if (zeroinit) {
            var ptr = ret, stop;
            assert((ret & 3) == 0);
            stop = ret + (size & ~3);
            for (; ptr < stop; ptr += 4) {
                HEAP32[ptr >> 2] = 0;
            }
            stop = ret + size;
            while (ptr < stop) {
                HEAP8[ptr++ >> 0] = 0;
            }
            return ret
        }
        if (singleType === "i8") {
            if (slab.subarray || slab.slice) {
                HEAPU8.set(slab, ret);
            } else {
                HEAPU8.set(new Uint8Array(slab), ret);
            }
            return ret
        }
        var i = 0, type, typeSize, previousType;
        while (i < size) {
            var curr = slab[i];
            if (typeof curr === "function") {
                curr = Runtime.getFunctionIndex(curr);
            }
            type = singleType || types[i];
            if (type === 0) {
                i++;
                continue
            }
            if (type == "i64") type = "i32";
            setValue(ret + i, curr, type);
            if (previousType !== type) {
                typeSize = Runtime.getNativeTypeSize(type);
                previousType = type;
            }
            i += typeSize;
        }
        return ret
    }

    Module["allocate"] = allocate;

    function getMemory(size) {
        if (!staticSealed) return Runtime.staticAlloc(size);
        if (!runtimeInitialized) return Runtime.dynamicAlloc(size);
        return _malloc(size)
    }

    Module["getMemory"] = getMemory;

    function Pointer_stringify(ptr, length) {
        if (length === 0 || !ptr) return "";
        var hasUtf = 0;
        var t;
        var i = 0;
        while (1) {
            t = HEAPU8[ptr + i >> 0];
            hasUtf |= t;
            if (t == 0 && !length) break;
            i++;
            if (length && i == length) break
        }
        if (!length) length = i;
        var ret = "";
        if (hasUtf < 128) {
            var MAX_CHUNK = 1024;
            var curr;
            while (length > 0) {
                curr = String.fromCharCode.apply(String, HEAPU8.subarray(ptr, ptr + Math.min(length, MAX_CHUNK)));
                ret = ret ? ret + curr : curr;
                ptr += MAX_CHUNK;
                length -= MAX_CHUNK;
            }
            return ret
        }
        return Module["UTF8ToString"](ptr)
    }

    Module["Pointer_stringify"] = Pointer_stringify;

    function AsciiToString(ptr) {
        var str = "";
        while (1) {
            var ch = HEAP8[ptr++ >> 0];
            if (!ch) return str;
            str += String.fromCharCode(ch);
        }
    }

    Module["AsciiToString"] = AsciiToString;

    function stringToAscii(str, outPtr) {
        return writeAsciiToMemory(str, outPtr, false)
    }

    Module["stringToAscii"] = stringToAscii;
    var UTF8Decoder = typeof TextDecoder !== "undefined" ? new TextDecoder("utf8") : undefined;

    function UTF8ArrayToString(u8Array, idx) {
        var endPtr = idx;
        while (u8Array[endPtr]) ++endPtr;
        if (endPtr - idx > 16 && u8Array.subarray && UTF8Decoder) {
            return UTF8Decoder.decode(u8Array.subarray(idx, endPtr))
        } else {
            var u0, u1, u2, u3, u4, u5;
            var str = "";
            while (1) {
                u0 = u8Array[idx++];
                if (!u0) return str;
                if (!(u0 & 128)) {
                    str += String.fromCharCode(u0);
                    continue
                }
                u1 = u8Array[idx++] & 63;
                if ((u0 & 224) == 192) {
                    str += String.fromCharCode((u0 & 31) << 6 | u1);
                    continue
                }
                u2 = u8Array[idx++] & 63;
                if ((u0 & 240) == 224) {
                    u0 = (u0 & 15) << 12 | u1 << 6 | u2;
                } else {
                    u3 = u8Array[idx++] & 63;
                    if ((u0 & 248) == 240) {
                        u0 = (u0 & 7) << 18 | u1 << 12 | u2 << 6 | u3;
                    } else {
                        u4 = u8Array[idx++] & 63;
                        if ((u0 & 252) == 248) {
                            u0 = (u0 & 3) << 24 | u1 << 18 | u2 << 12 | u3 << 6 | u4;
                        } else {
                            u5 = u8Array[idx++] & 63;
                            u0 = (u0 & 1) << 30 | u1 << 24 | u2 << 18 | u3 << 12 | u4 << 6 | u5;
                        }
                    }
                }
                if (u0 < 65536) {
                    str += String.fromCharCode(u0);
                } else {
                    var ch = u0 - 65536;
                    str += String.fromCharCode(55296 | ch >> 10, 56320 | ch & 1023);
                }
            }
        }
    }

    Module["UTF8ArrayToString"] = UTF8ArrayToString;

    function UTF8ToString(ptr) {
        return UTF8ArrayToString(HEAPU8, ptr)
    }

    Module["UTF8ToString"] = UTF8ToString;

    function stringToUTF8Array(str, outU8Array, outIdx, maxBytesToWrite) {
        if (!(maxBytesToWrite > 0)) return 0;
        var startIdx = outIdx;
        var endIdx = outIdx + maxBytesToWrite - 1;
        for (var i = 0; i < str.length; ++i) {
            var u = str.charCodeAt(i);
            if (u >= 55296 && u <= 57343) u = 65536 + ((u & 1023) << 10) | str.charCodeAt(++i) & 1023;
            if (u <= 127) {
                if (outIdx >= endIdx) break;
                outU8Array[outIdx++] = u;
            } else if (u <= 2047) {
                if (outIdx + 1 >= endIdx) break;
                outU8Array[outIdx++] = 192 | u >> 6;
                outU8Array[outIdx++] = 128 | u & 63;
            } else if (u <= 65535) {
                if (outIdx + 2 >= endIdx) break;
                outU8Array[outIdx++] = 224 | u >> 12;
                outU8Array[outIdx++] = 128 | u >> 6 & 63;
                outU8Array[outIdx++] = 128 | u & 63;
            } else if (u <= 2097151) {
                if (outIdx + 3 >= endIdx) break;
                outU8Array[outIdx++] = 240 | u >> 18;
                outU8Array[outIdx++] = 128 | u >> 12 & 63;
                outU8Array[outIdx++] = 128 | u >> 6 & 63;
                outU8Array[outIdx++] = 128 | u & 63;
            } else if (u <= 67108863) {
                if (outIdx + 4 >= endIdx) break;
                outU8Array[outIdx++] = 248 | u >> 24;
                outU8Array[outIdx++] = 128 | u >> 18 & 63;
                outU8Array[outIdx++] = 128 | u >> 12 & 63;
                outU8Array[outIdx++] = 128 | u >> 6 & 63;
                outU8Array[outIdx++] = 128 | u & 63;
            } else {
                if (outIdx + 5 >= endIdx) break;
                outU8Array[outIdx++] = 252 | u >> 30;
                outU8Array[outIdx++] = 128 | u >> 24 & 63;
                outU8Array[outIdx++] = 128 | u >> 18 & 63;
                outU8Array[outIdx++] = 128 | u >> 12 & 63;
                outU8Array[outIdx++] = 128 | u >> 6 & 63;
                outU8Array[outIdx++] = 128 | u & 63;
            }
        }
        outU8Array[outIdx] = 0;
        return outIdx - startIdx
    }

    Module["stringToUTF8Array"] = stringToUTF8Array;

    function stringToUTF8(str, outPtr, maxBytesToWrite) {
        return stringToUTF8Array(str, HEAPU8, outPtr, maxBytesToWrite)
    }

    Module["stringToUTF8"] = stringToUTF8;

    function lengthBytesUTF8(str) {
        var len = 0;
        for (var i = 0; i < str.length; ++i) {
            var u = str.charCodeAt(i);
            if (u >= 55296 && u <= 57343) u = 65536 + ((u & 1023) << 10) | str.charCodeAt(++i) & 1023;
            if (u <= 127) {
                ++len;
            } else if (u <= 2047) {
                len += 2;
            } else if (u <= 65535) {
                len += 3;
            } else if (u <= 2097151) {
                len += 4;
            } else if (u <= 67108863) {
                len += 5;
            } else {
                len += 6;
            }
        }
        return len
    }

    Module["lengthBytesUTF8"] = lengthBytesUTF8;
    var UTF16Decoder = typeof TextDecoder !== "undefined" ? new TextDecoder("utf-16le") : undefined;

    function demangle(func) {
        var __cxa_demangle_func = Module["___cxa_demangle"] || Module["__cxa_demangle"];
        if (__cxa_demangle_func) {
            try {
                var s = func.substr(1);
                var len = lengthBytesUTF8(s) + 1;
                var buf = _malloc(len);
                stringToUTF8(s, buf, len);
                var status = _malloc(4);
                var ret = __cxa_demangle_func(buf, 0, 0, status);
                if (getValue(status, "i32") === 0 && ret) {
                    return Pointer_stringify(ret)
                }
            } catch (e) {
            } finally {
                if (buf) _free(buf);
                if (status) _free(status);
                if (ret) _free(ret);
            }
            return func
        }
        Runtime.warnOnce("warning: build with  -s DEMANGLE_SUPPORT=1  to link in libcxxabi demangling");
        return func
    }

    function demangleAll(text) {
        var regex = /__Z[\w\d_]+/g;
        return text.replace(regex, (function (x) {
            var y = demangle(x);
            return x === y ? x : x + " [" + y + "]"
        }))
    }

    function jsStackTrace() {
        var err = new Error;
        if (!err.stack) {
            try {
                throw new Error(0)
            } catch (e) {
                err = e;
            }
            if (!err.stack) {
                return "(no stack trace available)"
            }
        }
        return err.stack.toString()
    }

    function stackTrace() {
        var js = jsStackTrace();
        if (Module["extraStackTrace"]) js += "\n" + Module["extraStackTrace"]();
        return demangleAll(js)
    }

    Module["stackTrace"] = stackTrace;
    var WASM_PAGE_SIZE = 65536;
    var ASMJS_PAGE_SIZE = 16777216;
    var MIN_TOTAL_MEMORY = 16777216;

    function alignUp(x, multiple) {
        if (x % multiple > 0) {
            x += multiple - x % multiple;
        }
        return x
    }

    var HEAP, buffer, HEAP8, HEAPU8, HEAP16, HEAPU16, HEAP32, HEAPU32, HEAPF32, HEAPF64;

    function updateGlobalBuffer(buf) {
        Module["buffer"] = buffer = buf;
    }

    function updateGlobalBufferViews() {
        Module["HEAP8"] = HEAP8 = new Int8Array(buffer);
        Module["HEAP16"] = HEAP16 = new Int16Array(buffer);
        Module["HEAP32"] = HEAP32 = new Int32Array(buffer);
        Module["HEAPU8"] = HEAPU8 = new Uint8Array(buffer);
        Module["HEAPU16"] = HEAPU16 = new Uint16Array(buffer);
        Module["HEAPU32"] = HEAPU32 = new Uint32Array(buffer);
        Module["HEAPF32"] = HEAPF32 = new Float32Array(buffer);
        Module["HEAPF64"] = HEAPF64 = new Float64Array(buffer);
    }

    var STATIC_BASE, STATICTOP, staticSealed;
    var STACK_BASE, STACKTOP, STACK_MAX;
    var DYNAMIC_BASE, DYNAMICTOP_PTR;
    STATIC_BASE = STATICTOP = STACK_BASE = STACKTOP = STACK_MAX = DYNAMIC_BASE = DYNAMICTOP_PTR = 0;
    staticSealed = false;

    function abortOnCannotGrowMemory() {
        abort("Cannot enlarge memory arrays. Either (1) compile with  -s TOTAL_MEMORY=X  with X higher than the current value " + TOTAL_MEMORY + ", (2) compile with  -s ALLOW_MEMORY_GROWTH=1  which allows increasing the size at runtime but prevents some optimizations, (3) set Module.TOTAL_MEMORY to a higher value before the program runs, or (4) if you want malloc to return NULL (0) instead of this abort, compile with  -s ABORTING_MALLOC=0 ");
    }

    if (!Module["reallocBuffer"]) Module["reallocBuffer"] = (function (size) {
        var ret;
        try {
            if (ArrayBuffer.transfer) {
                ret = ArrayBuffer.transfer(buffer, size);
            } else {
                var oldHEAP8 = HEAP8;
                ret = new ArrayBuffer(size);
                var temp = new Int8Array(ret);
                temp.set(oldHEAP8);
            }
        } catch (e) {
            return false
        }
        var success = _emscripten_replace_memory(ret);
        if (!success) return false;
        return ret
    });

    function enlargeMemory() {
        var PAGE_MULTIPLE = Module["usingWasm"] ? WASM_PAGE_SIZE : ASMJS_PAGE_SIZE;
        var LIMIT = 2147483648 - PAGE_MULTIPLE;
        if (HEAP32[DYNAMICTOP_PTR >> 2] > LIMIT) {
            return false
        }
        var OLD_TOTAL_MEMORY = TOTAL_MEMORY;
        TOTAL_MEMORY = Math.max(TOTAL_MEMORY, MIN_TOTAL_MEMORY);
        while (TOTAL_MEMORY < HEAP32[DYNAMICTOP_PTR >> 2]) {
            if (TOTAL_MEMORY <= 536870912) {
                TOTAL_MEMORY = alignUp(2 * TOTAL_MEMORY, PAGE_MULTIPLE);
            } else {
                TOTAL_MEMORY = Math.min(alignUp((3 * TOTAL_MEMORY + 2147483648) / 4, PAGE_MULTIPLE), LIMIT);
            }
        }
        var replacement = Module["reallocBuffer"](TOTAL_MEMORY);
        if (!replacement || replacement.byteLength != TOTAL_MEMORY) {
            TOTAL_MEMORY = OLD_TOTAL_MEMORY;
            return false
        }
        updateGlobalBuffer(replacement);
        updateGlobalBufferViews();
        return true
    }

    var byteLength;
    try {
        byteLength = Function.prototype.call.bind(Object.getOwnPropertyDescriptor(ArrayBuffer.prototype, "byteLength").get);
        byteLength(new ArrayBuffer(4));
    } catch (e) {
        byteLength = (function (buffer) {
            return buffer.byteLength
        });
    }
    var TOTAL_STACK = Module["TOTAL_STACK"] || 5242880;
    var TOTAL_MEMORY = Module["TOTAL_MEMORY"] || 16777216;
    if (TOTAL_MEMORY < TOTAL_STACK) Module.printErr("TOTAL_MEMORY should be larger than TOTAL_STACK, was " + TOTAL_MEMORY + "! (TOTAL_STACK=" + TOTAL_STACK + ")");
    if (Module["buffer"]) {
        buffer = Module["buffer"];
    } else {
        {
            buffer = new ArrayBuffer(TOTAL_MEMORY);
        }
    }
    updateGlobalBufferViews();

    function getTotalMemory() {
        return TOTAL_MEMORY
    }

    HEAP32[0] = 1668509029;
    HEAP16[1] = 25459;
    if (HEAPU8[2] !== 115 || HEAPU8[3] !== 99) throw "Runtime error: expected the system to be little-endian!";
    Module["HEAP"] = HEAP;
    Module["buffer"] = buffer;
    Module["HEAP8"] = HEAP8;
    Module["HEAP16"] = HEAP16;
    Module["HEAP32"] = HEAP32;
    Module["HEAPU8"] = HEAPU8;
    Module["HEAPU16"] = HEAPU16;
    Module["HEAPU32"] = HEAPU32;
    Module["HEAPF32"] = HEAPF32;
    Module["HEAPF64"] = HEAPF64;

    function callRuntimeCallbacks(callbacks) {
        while (callbacks.length > 0) {
            var callback = callbacks.shift();
            if (typeof callback == "function") {
                callback();
                continue
            }
            var func = callback.func;
            if (typeof func === "number") {
                if (callback.arg === undefined) {
                    Module["dynCall_v"](func);
                } else {
                    Module["dynCall_vi"](func, callback.arg);
                }
            } else {
                func(callback.arg === undefined ? null : callback.arg);
            }
        }
    }

    var __ATPRERUN__ = [];
    var __ATINIT__ = [];
    var __ATMAIN__ = [];
    var __ATEXIT__ = [];
    var __ATPOSTRUN__ = [];
    var runtimeInitialized = false;

    function preRun() {
        if (Module["preRun"]) {
            if (typeof Module["preRun"] == "function") Module["preRun"] = [Module["preRun"]];
            while (Module["preRun"].length) {
                addOnPreRun(Module["preRun"].shift());
            }
        }
        callRuntimeCallbacks(__ATPRERUN__);
    }

    function ensureInitRuntime() {
        if (runtimeInitialized) return;
        runtimeInitialized = true;
        callRuntimeCallbacks(__ATINIT__);
    }

    function preMain() {
        callRuntimeCallbacks(__ATMAIN__);
    }

    function exitRuntime() {
        callRuntimeCallbacks(__ATEXIT__);
    }

    function postRun() {
        if (Module["postRun"]) {
            if (typeof Module["postRun"] == "function") Module["postRun"] = [Module["postRun"]];
            while (Module["postRun"].length) {
                addOnPostRun(Module["postRun"].shift());
            }
        }
        callRuntimeCallbacks(__ATPOSTRUN__);
    }

    function addOnPreRun(cb) {
        __ATPRERUN__.unshift(cb);
    }

    Module["addOnPreRun"] = addOnPreRun;

    function addOnInit(cb) {
        __ATINIT__.unshift(cb);
    }

    Module["addOnInit"] = addOnInit;

    function addOnPreMain(cb) {
        __ATMAIN__.unshift(cb);
    }

    Module["addOnPreMain"] = addOnPreMain;

    function addOnExit(cb) {
        __ATEXIT__.unshift(cb);
    }

    Module["addOnExit"] = addOnExit;

    function addOnPostRun(cb) {
        __ATPOSTRUN__.unshift(cb);
    }

    Module["addOnPostRun"] = addOnPostRun;

    function intArrayFromString(stringy, dontAddNull, length) {
        var len = length > 0 ? length : lengthBytesUTF8(stringy) + 1;
        var u8array = new Array(len);
        var numBytesWritten = stringToUTF8Array(stringy, u8array, 0, u8array.length);
        if (dontAddNull) u8array.length = numBytesWritten;
        return u8array
    }

    Module["intArrayFromString"] = intArrayFromString;

    function intArrayToString(array) {
        var ret = [];
        for (var i = 0; i < array.length; i++) {
            var chr = array[i];
            if (chr > 255) {
                chr &= 255;
            }
            ret.push(String.fromCharCode(chr));
        }
        return ret.join("")
    }

    Module["intArrayToString"] = intArrayToString;

    function writeStringToMemory(string, buffer, dontAddNull) {
        Runtime.warnOnce("writeStringToMemory is deprecated and should not be called! Use stringToUTF8() instead!");
        var lastChar, end;
        if (dontAddNull) {
            end = buffer + lengthBytesUTF8(string);
            lastChar = HEAP8[end];
        }
        stringToUTF8(string, buffer, Infinity);
        if (dontAddNull) HEAP8[end] = lastChar;
    }

    Module["writeStringToMemory"] = writeStringToMemory;

    function writeArrayToMemory(array, buffer) {
        HEAP8.set(array, buffer);
    }

    Module["writeArrayToMemory"] = writeArrayToMemory;

    function writeAsciiToMemory(str, buffer, dontAddNull) {
        for (var i = 0; i < str.length; ++i) {
            HEAP8[buffer++ >> 0] = str.charCodeAt(i);
        }
        if (!dontAddNull) HEAP8[buffer >> 0] = 0;
    }

    Module["writeAsciiToMemory"] = writeAsciiToMemory;
    if (!Math["imul"] || Math["imul"](4294967295, 5) !== -5) Math["imul"] = function imul(a, b) {
        var ah = a >>> 16;
        var al = a & 65535;
        var bh = b >>> 16;
        var bl = b & 65535;
        return al * bl + (ah * bl + al * bh << 16) | 0
    };
    Math.imul = Math["imul"];
    if (!Math["clz32"]) Math["clz32"] = (function (x) {
        x = x >>> 0;
        for (var i = 0; i < 32; i++) {
            if (x & 1 << 31 - i) return i
        }
        return 32
    });
    Math.clz32 = Math["clz32"];
    if (!Math["trunc"]) Math["trunc"] = (function (x) {
        return x < 0 ? Math.ceil(x) : Math.floor(x)
    });
    Math.trunc = Math["trunc"];
    var Math_abs = Math.abs;
    var Math_ceil = Math.ceil;
    var Math_floor = Math.floor;
    var Math_min = Math.min;
    var runDependencies = 0;
    var dependenciesFulfilled = null;

    function addRunDependency(id) {
        runDependencies++;
        if (Module["monitorRunDependencies"]) {
            Module["monitorRunDependencies"](runDependencies);
        }
    }

    Module["addRunDependency"] = addRunDependency;

    function removeRunDependency(id) {
        runDependencies--;
        if (Module["monitorRunDependencies"]) {
            Module["monitorRunDependencies"](runDependencies);
        }
        if (runDependencies == 0) {
            if (dependenciesFulfilled) {
                var callback = dependenciesFulfilled;
                dependenciesFulfilled = null;
                callback();
            }
        }
    }

    Module["removeRunDependency"] = removeRunDependency;
    Module["preloadedImages"] = {};
    Module["preloadedAudios"] = {};
    STATIC_BASE = Runtime.GLOBAL_BASE;
    STATICTOP = STATIC_BASE + 6192;
    __ATINIT__.push();
    allocate([228, 2, 0, 0, 81, 16, 0, 0, 12, 3, 0, 0, 177, 16, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0, 12, 3, 0, 0, 94, 16, 0, 0, 48, 0, 0, 0, 0, 0, 0, 0, 228, 2, 0, 0, 127, 16, 0, 0, 12, 3, 0, 0, 140, 16, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 12, 3, 0, 0, 183, 17, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0, 12, 3, 0, 0, 147, 17, 0, 0, 72, 0, 0, 0, 0, 0, 0, 0, 108, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 32, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 248, 19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 224, 1, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 2, 0, 0, 0, 40, 20, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 255, 255, 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 56, 0, 0, 0, 1, 0, 0, 0, 5, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 37, 115, 40, 37, 117, 41, 58, 32, 65, 115, 115, 101, 114, 116, 105, 111, 110, 32, 102, 97, 105, 108, 117, 114, 101, 58, 32, 34, 37, 115, 34, 10, 0, 109, 95, 115, 105, 122, 101, 32, 60, 61, 32, 109, 95, 99, 97, 112, 97, 99, 105, 116, 121, 0, 46, 47, 105, 110, 99, 92, 99, 114, 110, 95, 100, 101, 99, 111, 109, 112, 46, 104, 0, 109, 105, 110, 95, 110, 101, 119, 95, 99, 97, 112, 97, 99, 105, 116, 121, 32, 60, 32, 40, 48, 120, 55, 70, 70, 70, 48, 48, 48, 48, 85, 32, 47, 32, 101, 108, 101, 109, 101, 110, 116, 95, 115, 105, 122, 101, 41, 0, 110, 101, 119, 95, 99, 97, 112, 97, 99, 105, 116, 121, 32, 38, 38, 32, 40, 110, 101, 119, 95, 99, 97, 112, 97, 99, 105, 116, 121, 32, 62, 32, 109, 95, 99, 97, 112, 97, 99, 105, 116, 121, 41, 0, 110, 117, 109, 95, 99, 111, 100, 101, 115, 91, 99, 93, 0, 115, 111, 114, 116, 101, 100, 95, 112, 111, 115, 32, 60, 32, 116, 111, 116, 97, 108, 95, 117, 115, 101, 100, 95, 115, 121, 109, 115, 0, 112, 67, 111, 100, 101, 115, 105, 122, 101, 115, 91, 115, 121, 109, 95, 105, 110, 100, 101, 120, 93, 32, 61, 61, 32, 99, 111, 100, 101, 115, 105, 122, 101, 0, 116, 32, 60, 32, 40, 49, 85, 32, 60, 60, 32, 116, 97, 98, 108, 101, 95, 98, 105, 116, 115, 41, 0, 109, 95, 108, 111, 111, 107, 117, 112, 91, 116, 93, 32, 61, 61, 32, 99, 85, 73, 78, 84, 51, 50, 95, 77, 65, 88, 0, 99, 114, 110, 100, 95, 109, 97, 108, 108, 111, 99, 58, 32, 115, 105, 122, 101, 32, 116, 111, 111, 32, 98, 105, 103, 0, 99, 114, 110, 100, 95, 109, 97, 108, 108, 111, 99, 58, 32, 111, 117, 116, 32, 111, 102, 32, 109, 101, 109, 111, 114, 121, 0, 40, 40, 117, 105, 110, 116, 51, 50, 41, 112, 95, 110, 101, 119, 32, 38, 32, 40, 67, 82, 78, 68, 95, 77, 73, 78, 95, 65, 76, 76, 79, 67, 95, 65, 76, 73, 71, 78, 77, 69, 78, 84, 32, 45, 32, 49, 41, 41, 32, 61, 61, 32, 48, 0, 99, 114, 110, 100, 95, 114, 101, 97, 108, 108, 111, 99, 58, 32, 98, 97, 100, 32, 112, 116, 114, 0, 99, 114, 110, 100, 95, 102, 114, 101, 101, 58, 32, 98, 97, 100, 32, 112, 116, 114, 0, 102, 97, 108, 115, 101, 0, 40, 116, 111, 116, 97, 108, 95, 115, 121, 109, 115, 32, 62, 61, 32, 49, 41, 32, 38, 38, 32, 40, 116, 111, 116, 97, 108, 95, 115, 121, 109, 115, 32, 60, 61, 32, 112, 114, 101, 102, 105, 120, 95, 99, 111, 100, 105, 110, 103, 58, 58, 99, 77, 97, 120, 83, 117, 112, 112, 111, 114, 116, 101, 100, 83, 121, 109, 115, 41, 0, 17, 18, 19, 20, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15, 16, 48, 0, 110, 117, 109, 95, 98, 105, 116, 115, 32, 60, 61, 32, 51, 50, 85, 0, 109, 95, 98, 105, 116, 95, 99, 111, 117, 110, 116, 32, 60, 61, 32, 99, 66, 105, 116, 66, 117, 102, 83, 105, 122, 101, 0, 116, 32, 33, 61, 32, 99, 85, 73, 78, 84, 51, 50, 95, 77, 65, 88, 0, 109, 111, 100, 101, 108, 46, 109, 95, 99, 111, 100, 101, 95, 115, 105, 122, 101, 115, 91, 115, 121, 109, 93, 32, 61, 61, 32, 108, 101, 110, 0, 0, 2, 3, 1, 0, 2, 3, 4, 5, 6, 7, 1, 40, 108, 101, 110, 32, 62, 61, 32, 49, 41, 32, 38, 38, 32, 40, 108, 101, 110, 32, 60, 61, 32, 99, 77, 97, 120, 69, 120, 112, 101, 99, 116, 101, 100, 67, 111, 100, 101, 83, 105, 122, 101, 41, 0, 105, 32, 60, 32, 109, 95, 115, 105, 122, 101, 0, 110, 101, 120, 116, 95, 108, 101, 118, 101, 108, 95, 111, 102, 115, 32, 62, 32, 99, 117, 114, 95, 108, 101, 118, 101, 108, 95, 111, 102, 115, 0, 1, 2, 2, 3, 3, 3, 3, 4, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 2, 1, 2, 0, 0, 0, 1, 0, 2, 1, 0, 2, 0, 0, 1, 2, 3, 110, 117, 109, 32, 38, 38, 32, 40, 110, 117, 109, 32, 61, 61, 32, 126, 110, 117, 109, 95, 99, 104, 101, 99, 107, 41, 0, 17, 0, 10, 0, 17, 17, 17, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 17, 0, 15, 10, 17, 17, 17, 3, 10, 7, 0, 1, 19, 9, 11, 11, 0, 0, 9, 6, 11, 0, 0, 11, 0, 6, 17, 0, 0, 0, 17, 17, 17, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 17, 0, 10, 10, 17, 17, 17, 0, 10, 0, 0, 2, 0, 9, 11, 0, 0, 0, 9, 0, 11, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 0, 0, 0, 0, 12, 0, 0, 0, 0, 9, 12, 0, 0, 0, 0, 0, 12, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13, 0, 0, 0, 4, 13, 0, 0, 0, 0, 9, 14, 0, 0, 0, 0, 0, 14, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0, 0, 15, 0, 0, 0, 0, 9, 16, 0, 0, 0, 0, 0, 16, 0, 0, 16, 0, 0, 18, 0, 0, 0, 18, 18, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 18, 0, 0, 0, 18, 18, 18, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 0, 10, 0, 0, 0, 0, 9, 11, 0, 0, 0, 0, 0, 11, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 0, 0, 0, 0, 12, 0, 0, 0, 0, 9, 12, 0, 0, 0, 0, 0, 12, 0, 0, 12, 0, 0, 45, 43, 32, 32, 32, 48, 88, 48, 120, 0, 40, 110, 117, 108, 108, 41, 0, 45, 48, 88, 43, 48, 88, 32, 48, 88, 45, 48, 120, 43, 48, 120, 32, 48, 120, 0, 105, 110, 102, 0, 73, 78, 70, 0, 110, 97, 110, 0, 78, 65, 78, 0, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 46, 0, 84, 33, 34, 25, 13, 1, 2, 3, 17, 75, 28, 12, 16, 4, 11, 29, 18, 30, 39, 104, 110, 111, 112, 113, 98, 32, 5, 6, 15, 19, 20, 21, 26, 8, 22, 7, 40, 36, 23, 24, 9, 10, 14, 27, 31, 37, 35, 131, 130, 125, 38, 42, 43, 60, 61, 62, 63, 67, 71, 74, 77, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 99, 100, 101, 102, 103, 105, 106, 107, 108, 114, 115, 116, 121, 122, 123, 124, 0, 73, 108, 108, 101, 103, 97, 108, 32, 98, 121, 116, 101, 32, 115, 101, 113, 117, 101, 110, 99, 101, 0, 68, 111, 109, 97, 105, 110, 32, 101, 114, 114, 111, 114, 0, 82, 101, 115, 117, 108, 116, 32, 110, 111, 116, 32, 114, 101, 112, 114, 101, 115, 101, 110, 116, 97, 98, 108, 101, 0, 78, 111, 116, 32, 97, 32, 116, 116, 121, 0, 80, 101, 114, 109, 105, 115, 115, 105, 111, 110, 32, 100, 101, 110, 105, 101, 100, 0, 79, 112, 101, 114, 97, 116, 105, 111, 110, 32, 110, 111, 116, 32, 112, 101, 114, 109, 105, 116, 116, 101, 100, 0, 78, 111, 32, 115, 117, 99, 104, 32, 102, 105, 108, 101, 32, 111, 114, 32, 100, 105, 114, 101, 99, 116, 111, 114, 121, 0, 78, 111, 32, 115, 117, 99, 104, 32, 112, 114, 111, 99, 101, 115, 115, 0, 70, 105, 108, 101, 32, 101, 120, 105, 115, 116, 115, 0, 86, 97, 108, 117, 101, 32, 116, 111, 111, 32, 108, 97, 114, 103, 101, 32, 102, 111, 114, 32, 100, 97, 116, 97, 32, 116, 121, 112, 101, 0, 78, 111, 32, 115, 112, 97, 99, 101, 32, 108, 101, 102, 116, 32, 111, 110, 32, 100, 101, 118, 105, 99, 101, 0, 79, 117, 116, 32, 111, 102, 32, 109, 101, 109, 111, 114, 121, 0, 82, 101, 115, 111, 117, 114, 99, 101, 32, 98, 117, 115, 121, 0, 73, 110, 116, 101, 114, 114, 117, 112, 116, 101, 100, 32, 115, 121, 115, 116, 101, 109, 32, 99, 97, 108, 108, 0, 82, 101, 115, 111, 117, 114, 99, 101, 32, 116, 101, 109, 112, 111, 114, 97, 114, 105, 108, 121, 32, 117, 110, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 73, 110, 118, 97, 108, 105, 100, 32, 115, 101, 101, 107, 0, 67, 114, 111, 115, 115, 45, 100, 101, 118, 105, 99, 101, 32, 108, 105, 110, 107, 0, 82, 101, 97, 100, 45, 111, 110, 108, 121, 32, 102, 105, 108, 101, 32, 115, 121, 115, 116, 101, 109, 0, 68, 105, 114, 101, 99, 116, 111, 114, 121, 32, 110, 111, 116, 32, 101, 109, 112, 116, 121, 0, 67, 111, 110, 110, 101, 99, 116, 105, 111, 110, 32, 114, 101, 115, 101, 116, 32, 98, 121, 32, 112, 101, 101, 114, 0, 79, 112, 101, 114, 97, 116, 105, 111, 110, 32, 116, 105, 109, 101, 100, 32, 111, 117, 116, 0, 67, 111, 110, 110, 101, 99, 116, 105, 111, 110, 32, 114, 101, 102, 117, 115, 101, 100, 0, 72, 111, 115, 116, 32, 105, 115, 32, 100, 111, 119, 110, 0, 72, 111, 115, 116, 32, 105, 115, 32, 117, 110, 114, 101, 97, 99, 104, 97, 98, 108, 101, 0, 65, 100, 100, 114, 101, 115, 115, 32, 105, 110, 32, 117, 115, 101, 0, 66, 114, 111, 107, 101, 110, 32, 112, 105, 112, 101, 0, 73, 47, 79, 32, 101, 114, 114, 111, 114, 0, 78, 111, 32, 115, 117, 99, 104, 32, 100, 101, 118, 105, 99, 101, 32, 111, 114, 32, 97, 100, 100, 114, 101, 115, 115, 0, 66, 108, 111, 99, 107, 32, 100, 101, 118, 105, 99, 101, 32, 114, 101, 113, 117, 105, 114, 101, 100, 0, 78, 111, 32, 115, 117, 99, 104, 32, 100, 101, 118, 105, 99, 101, 0, 78, 111, 116, 32, 97, 32, 100, 105, 114, 101, 99, 116, 111, 114, 121, 0, 73, 115, 32, 97, 32, 100, 105, 114, 101, 99, 116, 111, 114, 121, 0, 84, 101, 120, 116, 32, 102, 105, 108, 101, 32, 98, 117, 115, 121, 0, 69, 120, 101, 99, 32, 102, 111, 114, 109, 97, 116, 32, 101, 114, 114, 111, 114, 0, 73, 110, 118, 97, 108, 105, 100, 32, 97, 114, 103, 117, 109, 101, 110, 116, 0, 65, 114, 103, 117, 109, 101, 110, 116, 32, 108, 105, 115, 116, 32, 116, 111, 111, 32, 108, 111, 110, 103, 0, 83, 121, 109, 98, 111, 108, 105, 99, 32, 108, 105, 110, 107, 32, 108, 111, 111, 112, 0, 70, 105, 108, 101, 110, 97, 109, 101, 32, 116, 111, 111, 32, 108, 111, 110, 103, 0, 84, 111, 111, 32, 109, 97, 110, 121, 32, 111, 112, 101, 110, 32, 102, 105, 108, 101, 115, 32, 105, 110, 32, 115, 121, 115, 116, 101, 109, 0, 78, 111, 32, 102, 105, 108, 101, 32, 100, 101, 115, 99, 114, 105, 112, 116, 111, 114, 115, 32, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 66, 97, 100, 32, 102, 105, 108, 101, 32, 100, 101, 115, 99, 114, 105, 112, 116, 111, 114, 0, 78, 111, 32, 99, 104, 105, 108, 100, 32, 112, 114, 111, 99, 101, 115, 115, 0, 66, 97, 100, 32, 97, 100, 100, 114, 101, 115, 115, 0, 70, 105, 108, 101, 32, 116, 111, 111, 32, 108, 97, 114, 103, 101, 0, 84, 111, 111, 32, 109, 97, 110, 121, 32, 108, 105, 110, 107, 115, 0, 78, 111, 32, 108, 111, 99, 107, 115, 32, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 82, 101, 115, 111, 117, 114, 99, 101, 32, 100, 101, 97, 100, 108, 111, 99, 107, 32, 119, 111, 117, 108, 100, 32, 111, 99, 99, 117, 114, 0, 83, 116, 97, 116, 101, 32, 110, 111, 116, 32, 114, 101, 99, 111, 118, 101, 114, 97, 98, 108, 101, 0, 80, 114, 101, 118, 105, 111, 117, 115, 32, 111, 119, 110, 101, 114, 32, 100, 105, 101, 100, 0, 79, 112, 101, 114, 97, 116, 105, 111, 110, 32, 99, 97, 110, 99, 101, 108, 101, 100, 0, 70, 117, 110, 99, 116, 105, 111, 110, 32, 110, 111, 116, 32, 105, 109, 112, 108, 101, 109, 101, 110, 116, 101, 100, 0, 78, 111, 32, 109, 101, 115, 115, 97, 103, 101, 32, 111, 102, 32, 100, 101, 115, 105, 114, 101, 100, 32, 116, 121, 112, 101, 0, 73, 100, 101, 110, 116, 105, 102, 105, 101, 114, 32, 114, 101, 109, 111, 118, 101, 100, 0, 68, 101, 118, 105, 99, 101, 32, 110, 111, 116, 32, 97, 32, 115, 116, 114, 101, 97, 109, 0, 78, 111, 32, 100, 97, 116, 97, 32, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 68, 101, 118, 105, 99, 101, 32, 116, 105, 109, 101, 111, 117, 116, 0, 79, 117, 116, 32, 111, 102, 32, 115, 116, 114, 101, 97, 109, 115, 32, 114, 101, 115, 111, 117, 114, 99, 101, 115, 0, 76, 105, 110, 107, 32, 104, 97, 115, 32, 98, 101, 101, 110, 32, 115, 101, 118, 101, 114, 101, 100, 0, 80, 114, 111, 116, 111, 99, 111, 108, 32, 101, 114, 114, 111, 114, 0, 66, 97, 100, 32, 109, 101, 115, 115, 97, 103, 101, 0, 70, 105, 108, 101, 32, 100, 101, 115, 99, 114, 105, 112, 116, 111, 114, 32, 105, 110, 32, 98, 97, 100, 32, 115, 116, 97, 116, 101, 0, 78, 111, 116, 32, 97, 32, 115, 111, 99, 107, 101, 116, 0, 68, 101, 115, 116, 105, 110, 97, 116, 105, 111, 110, 32, 97, 100, 100, 114, 101, 115, 115, 32, 114, 101, 113, 117, 105, 114, 101, 100, 0, 77, 101, 115, 115, 97, 103, 101, 32, 116, 111, 111, 32, 108, 97, 114, 103, 101, 0, 80, 114, 111, 116, 111, 99, 111, 108, 32, 119, 114, 111, 110, 103, 32, 116, 121, 112, 101, 32, 102, 111, 114, 32, 115, 111, 99, 107, 101, 116, 0, 80, 114, 111, 116, 111, 99, 111, 108, 32, 110, 111, 116, 32, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 80, 114, 111, 116, 111, 99, 111, 108, 32, 110, 111, 116, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 0, 83, 111, 99, 107, 101, 116, 32, 116, 121, 112, 101, 32, 110, 111, 116, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 0, 78, 111, 116, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 0, 80, 114, 111, 116, 111, 99, 111, 108, 32, 102, 97, 109, 105, 108, 121, 32, 110, 111, 116, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 0, 65, 100, 100, 114, 101, 115, 115, 32, 102, 97, 109, 105, 108, 121, 32, 110, 111, 116, 32, 115, 117, 112, 112, 111, 114, 116, 101, 100, 32, 98, 121, 32, 112, 114, 111, 116, 111, 99, 111, 108, 0, 65, 100, 100, 114, 101, 115, 115, 32, 110, 111, 116, 32, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 78, 101, 116, 119, 111, 114, 107, 32, 105, 115, 32, 100, 111, 119, 110, 0, 78, 101, 116, 119, 111, 114, 107, 32, 117, 110, 114, 101, 97, 99, 104, 97, 98, 108, 101, 0, 67, 111, 110, 110, 101, 99, 116, 105, 111, 110, 32, 114, 101, 115, 101, 116, 32, 98, 121, 32, 110, 101, 116, 119, 111, 114, 107, 0, 67, 111, 110, 110, 101, 99, 116, 105, 111, 110, 32, 97, 98, 111, 114, 116, 101, 100, 0, 78, 111, 32, 98, 117, 102, 102, 101, 114, 32, 115, 112, 97, 99, 101, 32, 97, 118, 97, 105, 108, 97, 98, 108, 101, 0, 83, 111, 99, 107, 101, 116, 32, 105, 115, 32, 99, 111, 110, 110, 101, 99, 116, 101, 100, 0, 83, 111, 99, 107, 101, 116, 32, 110, 111, 116, 32, 99, 111, 110, 110, 101, 99, 116, 101, 100, 0, 67, 97, 110, 110, 111, 116, 32, 115, 101, 110, 100, 32, 97, 102, 116, 101, 114, 32, 115, 111, 99, 107, 101, 116, 32, 115, 104, 117, 116, 100, 111, 119, 110, 0, 79, 112, 101, 114, 97, 116, 105, 111, 110, 32, 97, 108, 114, 101, 97, 100, 121, 32, 105, 110, 32, 112, 114, 111, 103, 114, 101, 115, 115, 0, 79, 112, 101, 114, 97, 116, 105, 111, 110, 32, 105, 110, 32, 112, 114, 111, 103, 114, 101, 115, 115, 0, 83, 116, 97, 108, 101, 32, 102, 105, 108, 101, 32, 104, 97, 110, 100, 108, 101, 0, 82, 101, 109, 111, 116, 101, 32, 73, 47, 79, 32, 101, 114, 114, 111, 114, 0, 81, 117, 111, 116, 97, 32, 101, 120, 99, 101, 101, 100, 101, 100, 0, 78, 111, 32, 109, 101, 100, 105, 117, 109, 32, 102, 111, 117, 110, 100, 0, 87, 114, 111, 110, 103, 32, 109, 101, 100, 105, 117, 109, 32, 116, 121, 112, 101, 0, 78, 111, 32, 101, 114, 114, 111, 114, 32, 105, 110, 102, 111, 114, 109, 97, 116, 105, 111, 110, 0, 0, 116, 101, 114, 109, 105, 110, 97, 116, 105, 110, 103, 32, 119, 105, 116, 104, 32, 37, 115, 32, 101, 120, 99, 101, 112, 116, 105, 111, 110, 32, 111, 102, 32, 116, 121, 112, 101, 32, 37, 115, 58, 32, 37, 115, 0, 116, 101, 114, 109, 105, 110, 97, 116, 105, 110, 103, 32, 119, 105, 116, 104, 32, 37, 115, 32, 101, 120, 99, 101, 112, 116, 105, 111, 110, 32, 111, 102, 32, 116, 121, 112, 101, 32, 37, 115, 0, 116, 101, 114, 109, 105, 110, 97, 116, 105, 110, 103, 32, 119, 105, 116, 104, 32, 37, 115, 32, 102, 111, 114, 101, 105, 103, 110, 32, 101, 120, 99, 101, 112, 116, 105, 111, 110, 0, 116, 101, 114, 109, 105, 110, 97, 116, 105, 110, 103, 0, 117, 110, 99, 97, 117, 103, 104, 116, 0, 83, 116, 57, 101, 120, 99, 101, 112, 116, 105, 111, 110, 0, 78, 49, 48, 95, 95, 99, 120, 120, 97, 98, 105, 118, 49, 49, 54, 95, 95, 115, 104, 105, 109, 95, 116, 121, 112, 101, 95, 105, 110, 102, 111, 69, 0, 83, 116, 57, 116, 121, 112, 101, 95, 105, 110, 102, 111, 0, 78, 49, 48, 95, 95, 99, 120, 120, 97, 98, 105, 118, 49, 50, 48, 95, 95, 115, 105, 95, 99, 108, 97, 115, 115, 95, 116, 121, 112, 101, 95, 105, 110, 102, 111, 69, 0, 78, 49, 48, 95, 95, 99, 120, 120, 97, 98, 105, 118, 49, 49, 55, 95, 95, 99, 108, 97, 115, 115, 95, 116, 121, 112, 101, 95, 105, 110, 102, 111, 69, 0, 112, 116, 104, 114, 101, 97, 100, 95, 111, 110, 99, 101, 32, 102, 97, 105, 108, 117, 114, 101, 32, 105, 110, 32, 95, 95, 99, 120, 97, 95, 103, 101, 116, 95, 103, 108, 111, 98, 97, 108, 115, 95, 102, 97, 115, 116, 40, 41, 0, 99, 97, 110, 110, 111, 116, 32, 99, 114, 101, 97, 116, 101, 32, 112, 116, 104, 114, 101, 97, 100, 32, 107, 101, 121, 32, 102, 111, 114, 32, 95, 95, 99, 120, 97, 95, 103, 101, 116, 95, 103, 108, 111, 98, 97, 108, 115, 40, 41, 0, 99, 97, 110, 110, 111, 116, 32, 122, 101, 114, 111, 32, 111, 117, 116, 32, 116, 104, 114, 101, 97, 100, 32, 118, 97, 108, 117, 101, 32, 102, 111, 114, 32, 95, 95, 99, 120, 97, 95, 103, 101, 116, 95, 103, 108, 111, 98, 97, 108, 115, 40, 41, 0, 116, 101, 114, 109, 105, 110, 97, 116, 101, 95, 104, 97, 110, 100, 108, 101, 114, 32, 117, 110, 101, 120, 112, 101, 99, 116, 101, 100, 108, 121, 32, 114, 101, 116, 117, 114, 110, 101, 100, 0, 78, 49, 48, 95, 95, 99, 120, 120, 97, 98, 105, 118, 49, 49, 57, 95, 95, 112, 111, 105, 110, 116, 101, 114, 95, 116, 121, 112, 101, 95, 105, 110, 102, 111, 69, 0, 78, 49, 48, 95, 95, 99, 120, 120, 97, 98, 105, 118, 49, 49, 55, 95, 95, 112, 98, 97, 115, 101, 95, 116, 121, 112, 101, 95, 105, 110, 102, 111, 69, 0], "i8", ALLOC_NONE, Runtime.GLOBAL_BASE);
    var tempDoublePtr = STATICTOP;
    STATICTOP += 16;

    function _abort() {
        Module["abort"]();
    }

    function __ZSt18uncaught_exceptionv() {
        return !!__ZSt18uncaught_exceptionv.uncaught_exception
    }

    var EXCEPTIONS = {
        last: 0, caught: [], infos: {}, deAdjust: (function (adjusted) {
            if (!adjusted || EXCEPTIONS.infos[adjusted]) return adjusted;
            for (var ptr in EXCEPTIONS.infos) {
                var info = EXCEPTIONS.infos[ptr];
                if (info.adjusted === adjusted) {
                    return ptr
                }
            }
            return adjusted
        }), addRef: (function (ptr) {
            if (!ptr) return;
            var info = EXCEPTIONS.infos[ptr];
            info.refcount++;
        }), decRef: (function (ptr) {
            if (!ptr) return;
            var info = EXCEPTIONS.infos[ptr];
            assert(info.refcount > 0);
            info.refcount--;
            if (info.refcount === 0 && !info.rethrown) {
                if (info.destructor) {
                    Module["dynCall_vi"](info.destructor, ptr);
                }
                delete EXCEPTIONS.infos[ptr];
                ___cxa_free_exception(ptr);
            }
        }), clearRef: (function (ptr) {
            if (!ptr) return;
            var info = EXCEPTIONS.infos[ptr];
            info.refcount = 0;
        })
    };

    function ___cxa_begin_catch(ptr) {
        var info = EXCEPTIONS.infos[ptr];
        if (info && !info.caught) {
            info.caught = true;
            __ZSt18uncaught_exceptionv.uncaught_exception--;
        }
        if (info) info.rethrown = false;
        EXCEPTIONS.caught.push(ptr);
        EXCEPTIONS.addRef(EXCEPTIONS.deAdjust(ptr));
        return ptr
    }

    function _pthread_once(ptr, func) {
        if (!_pthread_once.seen) _pthread_once.seen = {};
        if (ptr in _pthread_once.seen) return;
        Module["dynCall_v"](func);
        _pthread_once.seen[ptr] = 1;
    }

    function _emscripten_memcpy_big(dest, src, num) {
        HEAPU8.set(HEAPU8.subarray(src, src + num), dest);
        return dest
    }

    var SYSCALLS = {
        varargs: 0, get: (function (varargs) {
            SYSCALLS.varargs += 4;
            var ret = HEAP32[SYSCALLS.varargs - 4 >> 2];
            return ret
        }), getStr: (function () {
            var ret = Pointer_stringify(SYSCALLS.get());
            return ret
        }), get64: (function () {
            var low = SYSCALLS.get(), high = SYSCALLS.get();
            if (low >= 0) assert(high === 0); else assert(high === -1);
            return low
        }), getZero: (function () {
            assert(SYSCALLS.get() === 0);
        })
    };

    function ___syscall6(which, varargs) {
        SYSCALLS.varargs = varargs;
        try {
            var stream = SYSCALLS.getStreamFromFD();
            FS.close(stream);
            return 0
        } catch (e) {
            if (typeof FS === "undefined" || !(e instanceof FS.ErrnoError)) abort(e);
            return -e.errno
        }
    }

    var cttz_i8 = allocate([8, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 6, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 7, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 6, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0], "i8", ALLOC_STATIC);
    var PTHREAD_SPECIFIC = {};

    function _pthread_getspecific(key) {
        return PTHREAD_SPECIFIC[key] || 0
    }

    function ___setErrNo(value) {
        if (Module["___errno_location"]) HEAP32[Module["___errno_location"]() >> 2] = value;
        return value
    }

    var PTHREAD_SPECIFIC_NEXT_KEY = 1;
    var ERRNO_CODES = {
        EPERM: 1,
        ENOENT: 2,
        ESRCH: 3,
        EINTR: 4,
        EIO: 5,
        ENXIO: 6,
        E2BIG: 7,
        ENOEXEC: 8,
        EBADF: 9,
        ECHILD: 10,
        EAGAIN: 11,
        EWOULDBLOCK: 11,
        ENOMEM: 12,
        EACCES: 13,
        EFAULT: 14,
        ENOTBLK: 15,
        EBUSY: 16,
        EEXIST: 17,
        EXDEV: 18,
        ENODEV: 19,
        ENOTDIR: 20,
        EISDIR: 21,
        EINVAL: 22,
        ENFILE: 23,
        EMFILE: 24,
        ENOTTY: 25,
        ETXTBSY: 26,
        EFBIG: 27,
        ENOSPC: 28,
        ESPIPE: 29,
        EROFS: 30,
        EMLINK: 31,
        EPIPE: 32,
        EDOM: 33,
        ERANGE: 34,
        ENOMSG: 42,
        EIDRM: 43,
        ECHRNG: 44,
        EL2NSYNC: 45,
        EL3HLT: 46,
        EL3RST: 47,
        ELNRNG: 48,
        EUNATCH: 49,
        ENOCSI: 50,
        EL2HLT: 51,
        EDEADLK: 35,
        ENOLCK: 37,
        EBADE: 52,
        EBADR: 53,
        EXFULL: 54,
        ENOANO: 55,
        EBADRQC: 56,
        EBADSLT: 57,
        EDEADLOCK: 35,
        EBFONT: 59,
        ENOSTR: 60,
        ENODATA: 61,
        ETIME: 62,
        ENOSR: 63,
        ENONET: 64,
        ENOPKG: 65,
        EREMOTE: 66,
        ENOLINK: 67,
        EADV: 68,
        ESRMNT: 69,
        ECOMM: 70,
        EPROTO: 71,
        EMULTIHOP: 72,
        EDOTDOT: 73,
        EBADMSG: 74,
        ENOTUNIQ: 76,
        EBADFD: 77,
        EREMCHG: 78,
        ELIBACC: 79,
        ELIBBAD: 80,
        ELIBSCN: 81,
        ELIBMAX: 82,
        ELIBEXEC: 83,
        ENOSYS: 38,
        ENOTEMPTY: 39,
        ENAMETOOLONG: 36,
        ELOOP: 40,
        EOPNOTSUPP: 95,
        EPFNOSUPPORT: 96,
        ECONNRESET: 104,
        ENOBUFS: 105,
        EAFNOSUPPORT: 97,
        EPROTOTYPE: 91,
        ENOTSOCK: 88,
        ENOPROTOOPT: 92,
        ESHUTDOWN: 108,
        ECONNREFUSED: 111,
        EADDRINUSE: 98,
        ECONNABORTED: 103,
        ENETUNREACH: 101,
        ENETDOWN: 100,
        ETIMEDOUT: 110,
        EHOSTDOWN: 112,
        EHOSTUNREACH: 113,
        EINPROGRESS: 115,
        EALREADY: 114,
        EDESTADDRREQ: 89,
        EMSGSIZE: 90,
        EPROTONOSUPPORT: 93,
        ESOCKTNOSUPPORT: 94,
        EADDRNOTAVAIL: 99,
        ENETRESET: 102,
        EISCONN: 106,
        ENOTCONN: 107,
        ETOOMANYREFS: 109,
        EUSERS: 87,
        EDQUOT: 122,
        ESTALE: 116,
        ENOTSUP: 95,
        ENOMEDIUM: 123,
        EILSEQ: 84,
        EOVERFLOW: 75,
        ECANCELED: 125,
        ENOTRECOVERABLE: 131,
        EOWNERDEAD: 130,
        ESTRPIPE: 86
    };

    function _pthread_key_create(key, destructor) {
        if (key == 0) {
            return ERRNO_CODES.EINVAL
        }
        HEAP32[key >> 2] = PTHREAD_SPECIFIC_NEXT_KEY;
        PTHREAD_SPECIFIC[PTHREAD_SPECIFIC_NEXT_KEY] = 0;
        PTHREAD_SPECIFIC_NEXT_KEY++;
        return 0
    }

    function ___resumeException(ptr) {
        if (!EXCEPTIONS.last) {
            EXCEPTIONS.last = ptr;
        }
        throw ptr + " - Exception catching is disabled, this exception cannot be caught. Compile with -s DISABLE_EXCEPTION_CATCHING=0 or DISABLE_EXCEPTION_CATCHING=2 to catch."
    }

    function ___cxa_find_matching_catch() {
        var thrown = EXCEPTIONS.last;
        if (!thrown) {
            return (Runtime.setTempRet0(0), 0) | 0
        }
        var info = EXCEPTIONS.infos[thrown];
        var throwntype = info.type;
        if (!throwntype) {
            return (Runtime.setTempRet0(0), thrown) | 0
        }
        var typeArray = Array.prototype.slice.call(arguments);
        var pointer = Module["___cxa_is_pointer_type"](throwntype);
        if (!___cxa_find_matching_catch.buffer) ___cxa_find_matching_catch.buffer = _malloc(4);
        HEAP32[___cxa_find_matching_catch.buffer >> 2] = thrown;
        thrown = ___cxa_find_matching_catch.buffer;
        for (var i = 0; i < typeArray.length; i++) {
            if (typeArray[i] && Module["___cxa_can_catch"](typeArray[i], throwntype, thrown)) {
                thrown = HEAP32[thrown >> 2];
                info.adjusted = thrown;
                return (Runtime.setTempRet0(typeArray[i]), thrown) | 0
            }
        }
        thrown = HEAP32[thrown >> 2];
        return (Runtime.setTempRet0(throwntype), thrown) | 0
    }

    function ___gxx_personality_v0() {
    }

    function _pthread_setspecific(key, value) {
        if (!(key in PTHREAD_SPECIFIC)) {
            return ERRNO_CODES.EINVAL
        }
        PTHREAD_SPECIFIC[key] = value;
        return 0
    }

    function ___syscall140(which, varargs) {
        SYSCALLS.varargs = varargs;
        try {
            var stream = SYSCALLS.getStreamFromFD(), offset_high = SYSCALLS.get(), offset_low = SYSCALLS.get(),
                result = SYSCALLS.get(), whence = SYSCALLS.get();
            var offset = offset_low;
            FS.llseek(stream, offset, whence);
            HEAP32[result >> 2] = stream.position;
            if (stream.getdents && offset === 0 && whence === 0) stream.getdents = null;
            return 0
        } catch (e) {
            if (typeof FS === "undefined" || !(e instanceof FS.ErrnoError)) abort(e);
            return -e.errno
        }
    }

    function ___syscall146(which, varargs) {
        SYSCALLS.varargs = varargs;
        try {
            var stream = SYSCALLS.get(), iov = SYSCALLS.get(), iovcnt = SYSCALLS.get();
            var ret = 0;
            if (!___syscall146.buffer) {
                ___syscall146.buffers = [null, [], []];
                ___syscall146.printChar = (function (stream, curr) {
                    var buffer = ___syscall146.buffers[stream];
                    assert(buffer);
                    if (curr === 0 || curr === 10) {
                        (stream === 1 ? Module["print"] : Module["printErr"])(UTF8ArrayToString(buffer, 0));
                        buffer.length = 0;
                    } else {
                        buffer.push(curr);
                    }
                });
            }
            for (var i = 0; i < iovcnt; i++) {
                var ptr = HEAP32[iov + i * 8 >> 2];
                var len = HEAP32[iov + (i * 8 + 4) >> 2];
                for (var j = 0; j < len; j++) {
                    ___syscall146.printChar(stream, HEAPU8[ptr + j]);
                }
                ret += len;
            }
            return ret
        } catch (e) {
            if (typeof FS === "undefined" || !(e instanceof FS.ErrnoError)) abort(e);
            return -e.errno
        }
    }

    function ___syscall54(which, varargs) {
        SYSCALLS.varargs = varargs;
        try {
            return 0
        } catch (e) {
            if (typeof FS === "undefined" || !(e instanceof FS.ErrnoError)) abort(e);
            return -e.errno
        }
    }

    __ATEXIT__.push((function () {
        var fflush = Module["_fflush"];
        if (fflush) fflush(0);
        var printChar = ___syscall146.printChar;
        if (!printChar) return;
        var buffers = ___syscall146.buffers;
        if (buffers[1].length) printChar(1, 10);
        if (buffers[2].length) printChar(2, 10);
    }));
    DYNAMICTOP_PTR = allocate(1, "i32", ALLOC_STATIC);
    STACK_BASE = STACKTOP = Runtime.alignMemory(STATICTOP);
    STACK_MAX = STACK_BASE + TOTAL_STACK;
    DYNAMIC_BASE = Runtime.alignMemory(STACK_MAX);
    HEAP32[DYNAMICTOP_PTR >> 2] = DYNAMIC_BASE;
    staticSealed = true;

    function invoke_iiii(index, a1, a2, a3) {
        try {
            return Module["dynCall_iiii"](index, a1, a2, a3)
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_viiiii(index, a1, a2, a3, a4, a5) {
        try {
            Module["dynCall_viiiii"](index, a1, a2, a3, a4, a5);
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_vi(index, a1) {
        try {
            Module["dynCall_vi"](index, a1);
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_ii(index, a1) {
        try {
            return Module["dynCall_ii"](index, a1)
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_viii(index, a1, a2, a3) {
        try {
            Module["dynCall_viii"](index, a1, a2, a3);
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_v(index) {
        try {
            Module["dynCall_v"](index);
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_viiiiii(index, a1, a2, a3, a4, a5, a6) {
        try {
            Module["dynCall_viiiiii"](index, a1, a2, a3, a4, a5, a6);
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    function invoke_viiii(index, a1, a2, a3, a4) {
        try {
            Module["dynCall_viiii"](index, a1, a2, a3, a4);
        } catch (e) {
            if (typeof e !== "number" && e !== "longjmp") throw e;
            Module["setThrew"](1, 0);
        }
    }

    Module.asmGlobalArg = {
        "Math": Math,
        "Int8Array": Int8Array,
        "Int16Array": Int16Array,
        "Int32Array": Int32Array,
        "Uint8Array": Uint8Array,
        "Uint16Array": Uint16Array,
        "Uint32Array": Uint32Array,
        "Float32Array": Float32Array,
        "Float64Array": Float64Array,
        "NaN": NaN,
        "Infinity": Infinity,
        "byteLength": byteLength
    };
    Module.asmLibraryArg = {
        "abort": abort,
        "assert": assert,
        "enlargeMemory": enlargeMemory,
        "getTotalMemory": getTotalMemory,
        "abortOnCannotGrowMemory": abortOnCannotGrowMemory,
        "invoke_iiii": invoke_iiii,
        "invoke_viiiii": invoke_viiiii,
        "invoke_vi": invoke_vi,
        "invoke_ii": invoke_ii,
        "invoke_viii": invoke_viii,
        "invoke_v": invoke_v,
        "invoke_viiiiii": invoke_viiiiii,
        "invoke_viiii": invoke_viiii,
        "_pthread_getspecific": _pthread_getspecific,
        "___syscall54": ___syscall54,
        "_pthread_setspecific": _pthread_setspecific,
        "___gxx_personality_v0": ___gxx_personality_v0,
        "___syscall6": ___syscall6,
        "___setErrNo": ___setErrNo,
        "_abort": _abort,
        "___cxa_begin_catch": ___cxa_begin_catch,
        "_pthread_once": _pthread_once,
        "_emscripten_memcpy_big": _emscripten_memcpy_big,
        "_pthread_key_create": _pthread_key_create,
        "___syscall140": ___syscall140,
        "___resumeException": ___resumeException,
        "___cxa_find_matching_catch": ___cxa_find_matching_catch,
        "___syscall146": ___syscall146,
        "__ZSt18uncaught_exceptionv": __ZSt18uncaught_exceptionv,
        "DYNAMICTOP_PTR": DYNAMICTOP_PTR,
        "tempDoublePtr": tempDoublePtr,
        "ABORT": ABORT,
        "STACKTOP": STACKTOP,
        "STACK_MAX": STACK_MAX,
        "cttz_i8": cttz_i8
    };// EMSCRIPTEN_START_ASM
    var asm = (function (global, env, buffer) {
        "almost asm";
        var a = global.Int8Array;
        var b = new a(buffer);
        var c = global.Int16Array;
        var d = new c(buffer);
        var e = global.Int32Array;
        var f = new e(buffer);
        var g = global.Uint8Array;
        var h = new g(buffer);
        var i = global.Uint16Array;
        var j = new i(buffer);
        var k = global.Uint32Array;
        var l = new k(buffer);
        var m = global.Float32Array;
        var n = new m(buffer);
        var o = global.Float64Array;
        var p = new o(buffer);
        var q = global.byteLength;
        var r = env.DYNAMICTOP_PTR | 0;
        var s = env.tempDoublePtr | 0;
        var t = env.ABORT | 0;
        var u = env.STACKTOP | 0;
        var v = env.STACK_MAX | 0;
        var w = env.cttz_i8 | 0;
        var B = global.NaN, C = global.Infinity;
        var I = 0;
        var J = global.Math.floor;
        var K = global.Math.abs;
        var L = global.Math.sqrt;
        var M = global.Math.pow;
        var N = global.Math.cos;
        var O = global.Math.sin;
        var P = global.Math.tan;
        var Q = global.Math.acos;
        var R = global.Math.asin;
        var S = global.Math.atan;
        var T = global.Math.atan2;
        var U = global.Math.exp;
        var V = global.Math.log;
        var W = global.Math.ceil;
        var X = global.Math.imul;
        var Y = global.Math.min;
        var Z = global.Math.max;
        var _ = global.Math.clz32;
        var $ = env.abort;
        var aa = env.assert;
        var ba = env.enlargeMemory;
        var ca = env.getTotalMemory;
        var da = env.abortOnCannotGrowMemory;
        var ea = env.invoke_iiii;
        var fa = env.invoke_viiiii;
        var ga = env.invoke_vi;
        var ha = env.invoke_ii;
        var ia = env.invoke_viii;
        var ja = env.invoke_v;
        var ka = env.invoke_viiiiii;
        var la = env.invoke_viiii;
        var ma = env._pthread_getspecific;
        var na = env.___syscall54;
        var oa = env._pthread_setspecific;
        var pa = env.___gxx_personality_v0;
        var qa = env.___syscall6;
        var ra = env.___setErrNo;
        var sa = env._abort;
        var ta = env.___cxa_begin_catch;
        var ua = env._pthread_once;
        var va = env._emscripten_memcpy_big;
        var wa = env._pthread_key_create;
        var xa = env.___syscall140;
        var ya = env.___resumeException;
        var za = env.___cxa_find_matching_catch;
        var Aa = env.___syscall146;
        var Ba = env.__ZSt18uncaught_exceptionv;

        function Da(newBuffer) {
            if (q(newBuffer) & 16777215 || q(newBuffer) <= 16777215 || q(newBuffer) > 2147483648) return false;
            b = new a(newBuffer);
            d = new c(newBuffer);
            f = new e(newBuffer);
            h = new g(newBuffer);
            j = new i(newBuffer);
            l = new k(newBuffer);
            n = new m(newBuffer);
            p = new o(newBuffer);
            buffer = newBuffer;
            return true
        }

        // EMSCRIPTEN_START_FUNCS
        function Ma(a) {
            a = a | 0;
            var b = 0, c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0,
                r = 0, s = 0, t = 0, v = 0, w = 0, x = 0;
            x = u;
            u = u + 16 | 0;
            n = x;
            do if (a >>> 0 < 245) {
                k = a >>> 0 < 11 ? 16 : a + 11 & -8;
                a = k >>> 3;
                m = f[1144] | 0;
                c = m >>> a;
                if (c & 3 | 0) {
                    b = (c & 1 ^ 1) + a | 0;
                    a = 4616 + (b << 1 << 2) | 0;
                    c = a + 8 | 0;
                    d = f[c >> 2] | 0;
                    e = d + 8 | 0;
                    g = f[e >> 2] | 0;
                    if ((a | 0) == (g | 0)) f[1144] = m & ~(1 << b); else {
                        f[g + 12 >> 2] = a;
                        f[c >> 2] = g;
                    }
                    w = b << 3;
                    f[d + 4 >> 2] = w | 3;
                    w = d + w + 4 | 0;
                    f[w >> 2] = f[w >> 2] | 1;
                    w = e;
                    u = x;
                    return w | 0
                }
                l = f[1146] | 0;
                if (k >>> 0 > l >>> 0) {
                    if (c | 0) {
                        b = 2 << a;
                        b = c << a & (b | 0 - b);
                        b = (b & 0 - b) + -1 | 0;
                        h = b >>> 12 & 16;
                        b = b >>> h;
                        c = b >>> 5 & 8;
                        b = b >>> c;
                        e = b >>> 2 & 4;
                        b = b >>> e;
                        a = b >>> 1 & 2;
                        b = b >>> a;
                        d = b >>> 1 & 1;
                        d = (c | h | e | a | d) + (b >>> d) | 0;
                        b = 4616 + (d << 1 << 2) | 0;
                        a = b + 8 | 0;
                        e = f[a >> 2] | 0;
                        h = e + 8 | 0;
                        c = f[h >> 2] | 0;
                        if ((b | 0) == (c | 0)) {
                            a = m & ~(1 << d);
                            f[1144] = a;
                        } else {
                            f[c + 12 >> 2] = b;
                            f[a >> 2] = c;
                            a = m;
                        }
                        g = (d << 3) - k | 0;
                        f[e + 4 >> 2] = k | 3;
                        d = e + k | 0;
                        f[d + 4 >> 2] = g | 1;
                        f[d + g >> 2] = g;
                        if (l | 0) {
                            e = f[1149] | 0;
                            b = l >>> 3;
                            c = 4616 + (b << 1 << 2) | 0;
                            b = 1 << b;
                            if (!(a & b)) {
                                f[1144] = a | b;
                                b = c;
                                a = c + 8 | 0;
                            } else {
                                a = c + 8 | 0;
                                b = f[a >> 2] | 0;
                            }
                            f[a >> 2] = e;
                            f[b + 12 >> 2] = e;
                            f[e + 8 >> 2] = b;
                            f[e + 12 >> 2] = c;
                        }
                        f[1146] = g;
                        f[1149] = d;
                        w = h;
                        u = x;
                        return w | 0
                    }
                    i = f[1145] | 0;
                    if (i) {
                        c = (i & 0 - i) + -1 | 0;
                        h = c >>> 12 & 16;
                        c = c >>> h;
                        g = c >>> 5 & 8;
                        c = c >>> g;
                        j = c >>> 2 & 4;
                        c = c >>> j;
                        d = c >>> 1 & 2;
                        c = c >>> d;
                        a = c >>> 1 & 1;
                        a = f[4880 + ((g | h | j | d | a) + (c >>> a) << 2) >> 2] | 0;
                        c = (f[a + 4 >> 2] & -8) - k | 0;
                        d = f[a + 16 + (((f[a + 16 >> 2] | 0) == 0 & 1) << 2) >> 2] | 0;
                        if (!d) {
                            j = a;
                            g = c;
                        } else {
                            do {
                                h = (f[d + 4 >> 2] & -8) - k | 0;
                                j = h >>> 0 < c >>> 0;
                                c = j ? h : c;
                                a = j ? d : a;
                                d = f[d + 16 + (((f[d + 16 >> 2] | 0) == 0 & 1) << 2) >> 2] | 0;
                            } while ((d | 0) != 0);
                            j = a;
                            g = c;
                        }
                        h = j + k | 0;
                        if (j >>> 0 < h >>> 0) {
                            e = f[j + 24 >> 2] | 0;
                            b = f[j + 12 >> 2] | 0;
                            do if ((b | 0) == (j | 0)) {
                                a = j + 20 | 0;
                                b = f[a >> 2] | 0;
                                if (!b) {
                                    a = j + 16 | 0;
                                    b = f[a >> 2] | 0;
                                    if (!b) {
                                        c = 0;
                                        break
                                    }
                                }
                                while (1) {
                                    c = b + 20 | 0;
                                    d = f[c >> 2] | 0;
                                    if (d | 0) {
                                        b = d;
                                        a = c;
                                        continue
                                    }
                                    c = b + 16 | 0;
                                    d = f[c >> 2] | 0;
                                    if (!d) break; else {
                                        b = d;
                                        a = c;
                                    }
                                }
                                f[a >> 2] = 0;
                                c = b;
                            } else {
                                c = f[j + 8 >> 2] | 0;
                                f[c + 12 >> 2] = b;
                                f[b + 8 >> 2] = c;
                                c = b;
                            } while (0);
                            do if (e | 0) {
                                b = f[j + 28 >> 2] | 0;
                                a = 4880 + (b << 2) | 0;
                                if ((j | 0) == (f[a >> 2] | 0)) {
                                    f[a >> 2] = c;
                                    if (!c) {
                                        f[1145] = i & ~(1 << b);
                                        break
                                    }
                                } else {
                                    f[e + 16 + (((f[e + 16 >> 2] | 0) != (j | 0) & 1) << 2) >> 2] = c;
                                    if (!c) break
                                }
                                f[c + 24 >> 2] = e;
                                b = f[j + 16 >> 2] | 0;
                                if (b | 0) {
                                    f[c + 16 >> 2] = b;
                                    f[b + 24 >> 2] = c;
                                }
                                b = f[j + 20 >> 2] | 0;
                                if (b | 0) {
                                    f[c + 20 >> 2] = b;
                                    f[b + 24 >> 2] = c;
                                }
                            } while (0);
                            if (g >>> 0 < 16) {
                                w = g + k | 0;
                                f[j + 4 >> 2] = w | 3;
                                w = j + w + 4 | 0;
                                f[w >> 2] = f[w >> 2] | 1;
                            } else {
                                f[j + 4 >> 2] = k | 3;
                                f[h + 4 >> 2] = g | 1;
                                f[h + g >> 2] = g;
                                if (l | 0) {
                                    d = f[1149] | 0;
                                    b = l >>> 3;
                                    c = 4616 + (b << 1 << 2) | 0;
                                    b = 1 << b;
                                    if (!(m & b)) {
                                        f[1144] = m | b;
                                        b = c;
                                        a = c + 8 | 0;
                                    } else {
                                        a = c + 8 | 0;
                                        b = f[a >> 2] | 0;
                                    }
                                    f[a >> 2] = d;
                                    f[b + 12 >> 2] = d;
                                    f[d + 8 >> 2] = b;
                                    f[d + 12 >> 2] = c;
                                }
                                f[1146] = g;
                                f[1149] = h;
                            }
                            w = j + 8 | 0;
                            u = x;
                            return w | 0
                        } else m = k;
                    } else m = k;
                } else m = k;
            } else if (a >>> 0 <= 4294967231) {
                a = a + 11 | 0;
                k = a & -8;
                j = f[1145] | 0;
                if (j) {
                    d = 0 - k | 0;
                    a = a >>> 8;
                    if (a) if (k >>> 0 > 16777215) i = 31; else {
                        m = (a + 1048320 | 0) >>> 16 & 8;
                        v = a << m;
                        l = (v + 520192 | 0) >>> 16 & 4;
                        v = v << l;
                        i = (v + 245760 | 0) >>> 16 & 2;
                        i = 14 - (l | m | i) + (v << i >>> 15) | 0;
                        i = k >>> (i + 7 | 0) & 1 | i << 1;
                    } else i = 0;
                    c = f[4880 + (i << 2) >> 2] | 0;
                    a:do if (!c) {
                        c = 0;
                        a = 0;
                        v = 57;
                    } else {
                        a = 0;
                        h = k << ((i | 0) == 31 ? 0 : 25 - (i >>> 1) | 0);
                        g = 0;
                        while (1) {
                            e = (f[c + 4 >> 2] & -8) - k | 0;
                            if (e >>> 0 < d >>> 0) if (!e) {
                                a = c;
                                d = 0;
                                e = c;
                                v = 61;
                                break a
                            } else {
                                a = c;
                                d = e;
                            }
                            e = f[c + 20 >> 2] | 0;
                            c = f[c + 16 + (h >>> 31 << 2) >> 2] | 0;
                            g = (e | 0) == 0 | (e | 0) == (c | 0) ? g : e;
                            e = (c | 0) == 0;
                            if (e) {
                                c = g;
                                v = 57;
                                break
                            } else h = h << ((e ^ 1) & 1);
                        }
                    } while (0);
                    if ((v | 0) == 57) {
                        if ((c | 0) == 0 & (a | 0) == 0) {
                            a = 2 << i;
                            a = j & (a | 0 - a);
                            if (!a) {
                                m = k;
                                break
                            }
                            m = (a & 0 - a) + -1 | 0;
                            h = m >>> 12 & 16;
                            m = m >>> h;
                            g = m >>> 5 & 8;
                            m = m >>> g;
                            i = m >>> 2 & 4;
                            m = m >>> i;
                            l = m >>> 1 & 2;
                            m = m >>> l;
                            c = m >>> 1 & 1;
                            a = 0;
                            c = f[4880 + ((g | h | i | l | c) + (m >>> c) << 2) >> 2] | 0;
                        }
                        if (!c) {
                            i = a;
                            h = d;
                        } else {
                            e = c;
                            v = 61;
                        }
                    }
                    if ((v | 0) == 61) while (1) {
                        v = 0;
                        c = (f[e + 4 >> 2] & -8) - k | 0;
                        m = c >>> 0 < d >>> 0;
                        c = m ? c : d;
                        a = m ? e : a;
                        e = f[e + 16 + (((f[e + 16 >> 2] | 0) == 0 & 1) << 2) >> 2] | 0;
                        if (!e) {
                            i = a;
                            h = c;
                            break
                        } else {
                            d = c;
                            v = 61;
                        }
                    }
                    if ((i | 0) != 0 ? h >>> 0 < ((f[1146] | 0) - k | 0) >>> 0 : 0) {
                        g = i + k | 0;
                        if (i >>> 0 >= g >>> 0) {
                            w = 0;
                            u = x;
                            return w | 0
                        }
                        e = f[i + 24 >> 2] | 0;
                        b = f[i + 12 >> 2] | 0;
                        do if ((b | 0) == (i | 0)) {
                            a = i + 20 | 0;
                            b = f[a >> 2] | 0;
                            if (!b) {
                                a = i + 16 | 0;
                                b = f[a >> 2] | 0;
                                if (!b) {
                                    b = 0;
                                    break
                                }
                            }
                            while (1) {
                                c = b + 20 | 0;
                                d = f[c >> 2] | 0;
                                if (d | 0) {
                                    b = d;
                                    a = c;
                                    continue
                                }
                                c = b + 16 | 0;
                                d = f[c >> 2] | 0;
                                if (!d) break; else {
                                    b = d;
                                    a = c;
                                }
                            }
                            f[a >> 2] = 0;
                        } else {
                            w = f[i + 8 >> 2] | 0;
                            f[w + 12 >> 2] = b;
                            f[b + 8 >> 2] = w;
                        } while (0);
                        do if (e) {
                            a = f[i + 28 >> 2] | 0;
                            c = 4880 + (a << 2) | 0;
                            if ((i | 0) == (f[c >> 2] | 0)) {
                                f[c >> 2] = b;
                                if (!b) {
                                    d = j & ~(1 << a);
                                    f[1145] = d;
                                    break
                                }
                            } else {
                                f[e + 16 + (((f[e + 16 >> 2] | 0) != (i | 0) & 1) << 2) >> 2] = b;
                                if (!b) {
                                    d = j;
                                    break
                                }
                            }
                            f[b + 24 >> 2] = e;
                            a = f[i + 16 >> 2] | 0;
                            if (a | 0) {
                                f[b + 16 >> 2] = a;
                                f[a + 24 >> 2] = b;
                            }
                            a = f[i + 20 >> 2] | 0;
                            if (a) {
                                f[b + 20 >> 2] = a;
                                f[a + 24 >> 2] = b;
                                d = j;
                            } else d = j;
                        } else d = j; while (0);
                        do if (h >>> 0 >= 16) {
                            f[i + 4 >> 2] = k | 3;
                            f[g + 4 >> 2] = h | 1;
                            f[g + h >> 2] = h;
                            b = h >>> 3;
                            if (h >>> 0 < 256) {
                                c = 4616 + (b << 1 << 2) | 0;
                                a = f[1144] | 0;
                                b = 1 << b;
                                if (!(a & b)) {
                                    f[1144] = a | b;
                                    b = c;
                                    a = c + 8 | 0;
                                } else {
                                    a = c + 8 | 0;
                                    b = f[a >> 2] | 0;
                                }
                                f[a >> 2] = g;
                                f[b + 12 >> 2] = g;
                                f[g + 8 >> 2] = b;
                                f[g + 12 >> 2] = c;
                                break
                            }
                            b = h >>> 8;
                            if (b) if (h >>> 0 > 16777215) b = 31; else {
                                v = (b + 1048320 | 0) >>> 16 & 8;
                                w = b << v;
                                t = (w + 520192 | 0) >>> 16 & 4;
                                w = w << t;
                                b = (w + 245760 | 0) >>> 16 & 2;
                                b = 14 - (t | v | b) + (w << b >>> 15) | 0;
                                b = h >>> (b + 7 | 0) & 1 | b << 1;
                            } else b = 0;
                            c = 4880 + (b << 2) | 0;
                            f[g + 28 >> 2] = b;
                            a = g + 16 | 0;
                            f[a + 4 >> 2] = 0;
                            f[a >> 2] = 0;
                            a = 1 << b;
                            if (!(d & a)) {
                                f[1145] = d | a;
                                f[c >> 2] = g;
                                f[g + 24 >> 2] = c;
                                f[g + 12 >> 2] = g;
                                f[g + 8 >> 2] = g;
                                break
                            }
                            a = h << ((b | 0) == 31 ? 0 : 25 - (b >>> 1) | 0);
                            c = f[c >> 2] | 0;
                            while (1) {
                                if ((f[c + 4 >> 2] & -8 | 0) == (h | 0)) {
                                    v = 97;
                                    break
                                }
                                d = c + 16 + (a >>> 31 << 2) | 0;
                                b = f[d >> 2] | 0;
                                if (!b) {
                                    v = 96;
                                    break
                                } else {
                                    a = a << 1;
                                    c = b;
                                }
                            }
                            if ((v | 0) == 96) {
                                f[d >> 2] = g;
                                f[g + 24 >> 2] = c;
                                f[g + 12 >> 2] = g;
                                f[g + 8 >> 2] = g;
                                break
                            } else if ((v | 0) == 97) {
                                v = c + 8 | 0;
                                w = f[v >> 2] | 0;
                                f[w + 12 >> 2] = g;
                                f[v >> 2] = g;
                                f[g + 8 >> 2] = w;
                                f[g + 12 >> 2] = c;
                                f[g + 24 >> 2] = 0;
                                break
                            }
                        } else {
                            w = h + k | 0;
                            f[i + 4 >> 2] = w | 3;
                            w = i + w + 4 | 0;
                            f[w >> 2] = f[w >> 2] | 1;
                        } while (0);
                        w = i + 8 | 0;
                        u = x;
                        return w | 0
                    } else m = k;
                } else m = k;
            } else m = -1; while (0);
            c = f[1146] | 0;
            if (c >>> 0 >= m >>> 0) {
                b = c - m | 0;
                a = f[1149] | 0;
                if (b >>> 0 > 15) {
                    w = a + m | 0;
                    f[1149] = w;
                    f[1146] = b;
                    f[w + 4 >> 2] = b | 1;
                    f[w + b >> 2] = b;
                    f[a + 4 >> 2] = m | 3;
                } else {
                    f[1146] = 0;
                    f[1149] = 0;
                    f[a + 4 >> 2] = c | 3;
                    w = a + c + 4 | 0;
                    f[w >> 2] = f[w >> 2] | 1;
                }
                w = a + 8 | 0;
                u = x;
                return w | 0
            }
            h = f[1147] | 0;
            if (h >>> 0 > m >>> 0) {
                t = h - m | 0;
                f[1147] = t;
                w = f[1150] | 0;
                v = w + m | 0;
                f[1150] = v;
                f[v + 4 >> 2] = t | 1;
                f[w + 4 >> 2] = m | 3;
                w = w + 8 | 0;
                u = x;
                return w | 0
            }
            if (!(f[1262] | 0)) {
                f[1264] = 4096;
                f[1263] = 4096;
                f[1265] = -1;
                f[1266] = -1;
                f[1267] = 0;
                f[1255] = 0;
                a = n & -16 ^ 1431655768;
                f[n >> 2] = a;
                f[1262] = a;
                a = 4096;
            } else a = f[1264] | 0;
            i = m + 48 | 0;
            j = m + 47 | 0;
            g = a + j | 0;
            e = 0 - a | 0;
            k = g & e;
            if (k >>> 0 <= m >>> 0) {
                w = 0;
                u = x;
                return w | 0
            }
            a = f[1254] | 0;
            if (a | 0 ? (l = f[1252] | 0, n = l + k | 0, n >>> 0 <= l >>> 0 | n >>> 0 > a >>> 0) : 0) {
                w = 0;
                u = x;
                return w | 0
            }
            b:do if (!(f[1255] & 4)) {
                c = f[1150] | 0;
                c:do if (c) {
                    d = 5024;
                    while (1) {
                        a = f[d >> 2] | 0;
                        if (a >>> 0 <= c >>> 0 ? (q = d + 4 | 0, (a + (f[q >> 2] | 0) | 0) >>> 0 > c >>> 0) : 0) break;
                        a = f[d + 8 >> 2] | 0;
                        if (!a) {
                            v = 118;
                            break c
                        } else d = a;
                    }
                    b = g - h & e;
                    if (b >>> 0 < 2147483647) {
                        a = ac(b | 0) | 0;
                        if ((a | 0) == ((f[d >> 2] | 0) + (f[q >> 2] | 0) | 0)) {
                            if ((a | 0) != (-1 | 0)) {
                                h = b;
                                g = a;
                                v = 135;
                                break b
                            }
                        } else {
                            d = a;
                            v = 126;
                        }
                    } else b = 0;
                } else v = 118; while (0);
                do if ((v | 0) == 118) {
                    c = ac(0) | 0;
                    if ((c | 0) != (-1 | 0) ? (b = c, o = f[1263] | 0, p = o + -1 | 0, b = ((p & b | 0) == 0 ? 0 : (p + b & 0 - o) - b | 0) + k | 0, o = f[1252] | 0, p = b + o | 0, b >>> 0 > m >>> 0 & b >>> 0 < 2147483647) : 0) {
                        q = f[1254] | 0;
                        if (q | 0 ? p >>> 0 <= o >>> 0 | p >>> 0 > q >>> 0 : 0) {
                            b = 0;
                            break
                        }
                        a = ac(b | 0) | 0;
                        if ((a | 0) == (c | 0)) {
                            h = b;
                            g = c;
                            v = 135;
                            break b
                        } else {
                            d = a;
                            v = 126;
                        }
                    } else b = 0;
                } while (0);
                do if ((v | 0) == 126) {
                    c = 0 - b | 0;
                    if (!(i >>> 0 > b >>> 0 & (b >>> 0 < 2147483647 & (d | 0) != (-1 | 0)))) if ((d | 0) == (-1 | 0)) {
                        b = 0;
                        break
                    } else {
                        h = b;
                        g = d;
                        v = 135;
                        break b
                    }
                    a = f[1264] | 0;
                    a = j - b + a & 0 - a;
                    if (a >>> 0 >= 2147483647) {
                        h = b;
                        g = d;
                        v = 135;
                        break b
                    }
                    if ((ac(a | 0) | 0) == (-1 | 0)) {
                        ac(c | 0) | 0;
                        b = 0;
                        break
                    } else {
                        h = a + b | 0;
                        g = d;
                        v = 135;
                        break b
                    }
                } while (0);
                f[1255] = f[1255] | 4;
                v = 133;
            } else {
                b = 0;
                v = 133;
            } while (0);
            if (((v | 0) == 133 ? k >>> 0 < 2147483647 : 0) ? (t = ac(k | 0) | 0, q = ac(0) | 0, r = q - t | 0, s = r >>> 0 > (m + 40 | 0) >>> 0, !((t | 0) == (-1 | 0) | s ^ 1 | t >>> 0 < q >>> 0 & ((t | 0) != (-1 | 0) & (q | 0) != (-1 | 0)) ^ 1)) : 0) {
                h = s ? r : b;
                g = t;
                v = 135;
            }
            if ((v | 0) == 135) {
                b = (f[1252] | 0) + h | 0;
                f[1252] = b;
                if (b >>> 0 > (f[1253] | 0) >>> 0) f[1253] = b;
                j = f[1150] | 0;
                do if (j) {
                    b = 5024;
                    while (1) {
                        a = f[b >> 2] | 0;
                        c = b + 4 | 0;
                        d = f[c >> 2] | 0;
                        if ((g | 0) == (a + d | 0)) {
                            v = 145;
                            break
                        }
                        e = f[b + 8 >> 2] | 0;
                        if (!e) break; else b = e;
                    }
                    if (((v | 0) == 145 ? (f[b + 12 >> 2] & 8 | 0) == 0 : 0) ? j >>> 0 < g >>> 0 & j >>> 0 >= a >>> 0 : 0) {
                        f[c >> 2] = d + h;
                        w = j + 8 | 0;
                        w = (w & 7 | 0) == 0 ? 0 : 0 - w & 7;
                        v = j + w | 0;
                        w = (f[1147] | 0) + (h - w) | 0;
                        f[1150] = v;
                        f[1147] = w;
                        f[v + 4 >> 2] = w | 1;
                        f[v + w + 4 >> 2] = 40;
                        f[1151] = f[1266];
                        break
                    }
                    if (g >>> 0 < (f[1148] | 0) >>> 0) f[1148] = g;
                    c = g + h | 0;
                    b = 5024;
                    while (1) {
                        if ((f[b >> 2] | 0) == (c | 0)) {
                            v = 153;
                            break
                        }
                        a = f[b + 8 >> 2] | 0;
                        if (!a) break; else b = a;
                    }
                    if ((v | 0) == 153 ? (f[b + 12 >> 2] & 8 | 0) == 0 : 0) {
                        f[b >> 2] = g;
                        l = b + 4 | 0;
                        f[l >> 2] = (f[l >> 2] | 0) + h;
                        l = g + 8 | 0;
                        l = g + ((l & 7 | 0) == 0 ? 0 : 0 - l & 7) | 0;
                        b = c + 8 | 0;
                        b = c + ((b & 7 | 0) == 0 ? 0 : 0 - b & 7) | 0;
                        k = l + m | 0;
                        i = b - l - m | 0;
                        f[l + 4 >> 2] = m | 3;
                        do if ((b | 0) != (j | 0)) {
                            if ((b | 0) == (f[1149] | 0)) {
                                w = (f[1146] | 0) + i | 0;
                                f[1146] = w;
                                f[1149] = k;
                                f[k + 4 >> 2] = w | 1;
                                f[k + w >> 2] = w;
                                break
                            }
                            a = f[b + 4 >> 2] | 0;
                            if ((a & 3 | 0) == 1) {
                                h = a & -8;
                                d = a >>> 3;
                                d:do if (a >>> 0 < 256) {
                                    a = f[b + 8 >> 2] | 0;
                                    c = f[b + 12 >> 2] | 0;
                                    if ((c | 0) == (a | 0)) {
                                        f[1144] = f[1144] & ~(1 << d);
                                        break
                                    } else {
                                        f[a + 12 >> 2] = c;
                                        f[c + 8 >> 2] = a;
                                        break
                                    }
                                } else {
                                    g = f[b + 24 >> 2] | 0;
                                    a = f[b + 12 >> 2] | 0;
                                    do if ((a | 0) == (b | 0)) {
                                        d = b + 16 | 0;
                                        c = d + 4 | 0;
                                        a = f[c >> 2] | 0;
                                        if (!a) {
                                            a = f[d >> 2] | 0;
                                            if (!a) {
                                                a = 0;
                                                break
                                            } else c = d;
                                        }
                                        while (1) {
                                            d = a + 20 | 0;
                                            e = f[d >> 2] | 0;
                                            if (e | 0) {
                                                a = e;
                                                c = d;
                                                continue
                                            }
                                            d = a + 16 | 0;
                                            e = f[d >> 2] | 0;
                                            if (!e) break; else {
                                                a = e;
                                                c = d;
                                            }
                                        }
                                        f[c >> 2] = 0;
                                    } else {
                                        w = f[b + 8 >> 2] | 0;
                                        f[w + 12 >> 2] = a;
                                        f[a + 8 >> 2] = w;
                                    } while (0);
                                    if (!g) break;
                                    c = f[b + 28 >> 2] | 0;
                                    d = 4880 + (c << 2) | 0;
                                    do if ((b | 0) != (f[d >> 2] | 0)) {
                                        f[g + 16 + (((f[g + 16 >> 2] | 0) != (b | 0) & 1) << 2) >> 2] = a;
                                        if (!a) break d
                                    } else {
                                        f[d >> 2] = a;
                                        if (a | 0) break;
                                        f[1145] = f[1145] & ~(1 << c);
                                        break d
                                    } while (0);
                                    f[a + 24 >> 2] = g;
                                    c = b + 16 | 0;
                                    d = f[c >> 2] | 0;
                                    if (d | 0) {
                                        f[a + 16 >> 2] = d;
                                        f[d + 24 >> 2] = a;
                                    }
                                    c = f[c + 4 >> 2] | 0;
                                    if (!c) break;
                                    f[a + 20 >> 2] = c;
                                    f[c + 24 >> 2] = a;
                                } while (0);
                                b = b + h | 0;
                                e = h + i | 0;
                            } else e = i;
                            b = b + 4 | 0;
                            f[b >> 2] = f[b >> 2] & -2;
                            f[k + 4 >> 2] = e | 1;
                            f[k + e >> 2] = e;
                            b = e >>> 3;
                            if (e >>> 0 < 256) {
                                c = 4616 + (b << 1 << 2) | 0;
                                a = f[1144] | 0;
                                b = 1 << b;
                                if (!(a & b)) {
                                    f[1144] = a | b;
                                    b = c;
                                    a = c + 8 | 0;
                                } else {
                                    a = c + 8 | 0;
                                    b = f[a >> 2] | 0;
                                }
                                f[a >> 2] = k;
                                f[b + 12 >> 2] = k;
                                f[k + 8 >> 2] = b;
                                f[k + 12 >> 2] = c;
                                break
                            }
                            b = e >>> 8;
                            do if (!b) b = 0; else {
                                if (e >>> 0 > 16777215) {
                                    b = 31;
                                    break
                                }
                                v = (b + 1048320 | 0) >>> 16 & 8;
                                w = b << v;
                                t = (w + 520192 | 0) >>> 16 & 4;
                                w = w << t;
                                b = (w + 245760 | 0) >>> 16 & 2;
                                b = 14 - (t | v | b) + (w << b >>> 15) | 0;
                                b = e >>> (b + 7 | 0) & 1 | b << 1;
                            } while (0);
                            d = 4880 + (b << 2) | 0;
                            f[k + 28 >> 2] = b;
                            a = k + 16 | 0;
                            f[a + 4 >> 2] = 0;
                            f[a >> 2] = 0;
                            a = f[1145] | 0;
                            c = 1 << b;
                            if (!(a & c)) {
                                f[1145] = a | c;
                                f[d >> 2] = k;
                                f[k + 24 >> 2] = d;
                                f[k + 12 >> 2] = k;
                                f[k + 8 >> 2] = k;
                                break
                            }
                            a = e << ((b | 0) == 31 ? 0 : 25 - (b >>> 1) | 0);
                            c = f[d >> 2] | 0;
                            while (1) {
                                if ((f[c + 4 >> 2] & -8 | 0) == (e | 0)) {
                                    v = 194;
                                    break
                                }
                                d = c + 16 + (a >>> 31 << 2) | 0;
                                b = f[d >> 2] | 0;
                                if (!b) {
                                    v = 193;
                                    break
                                } else {
                                    a = a << 1;
                                    c = b;
                                }
                            }
                            if ((v | 0) == 193) {
                                f[d >> 2] = k;
                                f[k + 24 >> 2] = c;
                                f[k + 12 >> 2] = k;
                                f[k + 8 >> 2] = k;
                                break
                            } else if ((v | 0) == 194) {
                                v = c + 8 | 0;
                                w = f[v >> 2] | 0;
                                f[w + 12 >> 2] = k;
                                f[v >> 2] = k;
                                f[k + 8 >> 2] = w;
                                f[k + 12 >> 2] = c;
                                f[k + 24 >> 2] = 0;
                                break
                            }
                        } else {
                            w = (f[1147] | 0) + i | 0;
                            f[1147] = w;
                            f[1150] = k;
                            f[k + 4 >> 2] = w | 1;
                        } while (0);
                        w = l + 8 | 0;
                        u = x;
                        return w | 0
                    }
                    b = 5024;
                    while (1) {
                        a = f[b >> 2] | 0;
                        if (a >>> 0 <= j >>> 0 ? (w = a + (f[b + 4 >> 2] | 0) | 0, w >>> 0 > j >>> 0) : 0) break;
                        b = f[b + 8 >> 2] | 0;
                    }
                    e = w + -47 | 0;
                    a = e + 8 | 0;
                    a = e + ((a & 7 | 0) == 0 ? 0 : 0 - a & 7) | 0;
                    e = j + 16 | 0;
                    a = a >>> 0 < e >>> 0 ? j : a;
                    b = a + 8 | 0;
                    c = g + 8 | 0;
                    c = (c & 7 | 0) == 0 ? 0 : 0 - c & 7;
                    v = g + c | 0;
                    c = h + -40 - c | 0;
                    f[1150] = v;
                    f[1147] = c;
                    f[v + 4 >> 2] = c | 1;
                    f[v + c + 4 >> 2] = 40;
                    f[1151] = f[1266];
                    c = a + 4 | 0;
                    f[c >> 2] = 27;
                    f[b >> 2] = f[1256];
                    f[b + 4 >> 2] = f[1257];
                    f[b + 8 >> 2] = f[1258];
                    f[b + 12 >> 2] = f[1259];
                    f[1256] = g;
                    f[1257] = h;
                    f[1259] = 0;
                    f[1258] = b;
                    b = a + 24 | 0;
                    do {
                        v = b;
                        b = b + 4 | 0;
                        f[b >> 2] = 7;
                    } while ((v + 8 | 0) >>> 0 < w >>> 0);
                    if ((a | 0) != (j | 0)) {
                        g = a - j | 0;
                        f[c >> 2] = f[c >> 2] & -2;
                        f[j + 4 >> 2] = g | 1;
                        f[a >> 2] = g;
                        b = g >>> 3;
                        if (g >>> 0 < 256) {
                            c = 4616 + (b << 1 << 2) | 0;
                            a = f[1144] | 0;
                            b = 1 << b;
                            if (!(a & b)) {
                                f[1144] = a | b;
                                b = c;
                                a = c + 8 | 0;
                            } else {
                                a = c + 8 | 0;
                                b = f[a >> 2] | 0;
                            }
                            f[a >> 2] = j;
                            f[b + 12 >> 2] = j;
                            f[j + 8 >> 2] = b;
                            f[j + 12 >> 2] = c;
                            break
                        }
                        b = g >>> 8;
                        if (b) if (g >>> 0 > 16777215) c = 31; else {
                            v = (b + 1048320 | 0) >>> 16 & 8;
                            w = b << v;
                            t = (w + 520192 | 0) >>> 16 & 4;
                            w = w << t;
                            c = (w + 245760 | 0) >>> 16 & 2;
                            c = 14 - (t | v | c) + (w << c >>> 15) | 0;
                            c = g >>> (c + 7 | 0) & 1 | c << 1;
                        } else c = 0;
                        d = 4880 + (c << 2) | 0;
                        f[j + 28 >> 2] = c;
                        f[j + 20 >> 2] = 0;
                        f[e >> 2] = 0;
                        b = f[1145] | 0;
                        a = 1 << c;
                        if (!(b & a)) {
                            f[1145] = b | a;
                            f[d >> 2] = j;
                            f[j + 24 >> 2] = d;
                            f[j + 12 >> 2] = j;
                            f[j + 8 >> 2] = j;
                            break
                        }
                        a = g << ((c | 0) == 31 ? 0 : 25 - (c >>> 1) | 0);
                        c = f[d >> 2] | 0;
                        while (1) {
                            if ((f[c + 4 >> 2] & -8 | 0) == (g | 0)) {
                                v = 216;
                                break
                            }
                            d = c + 16 + (a >>> 31 << 2) | 0;
                            b = f[d >> 2] | 0;
                            if (!b) {
                                v = 215;
                                break
                            } else {
                                a = a << 1;
                                c = b;
                            }
                        }
                        if ((v | 0) == 215) {
                            f[d >> 2] = j;
                            f[j + 24 >> 2] = c;
                            f[j + 12 >> 2] = j;
                            f[j + 8 >> 2] = j;
                            break
                        } else if ((v | 0) == 216) {
                            v = c + 8 | 0;
                            w = f[v >> 2] | 0;
                            f[w + 12 >> 2] = j;
                            f[v >> 2] = j;
                            f[j + 8 >> 2] = w;
                            f[j + 12 >> 2] = c;
                            f[j + 24 >> 2] = 0;
                            break
                        }
                    }
                } else {
                    w = f[1148] | 0;
                    if ((w | 0) == 0 | g >>> 0 < w >>> 0) f[1148] = g;
                    f[1256] = g;
                    f[1257] = h;
                    f[1259] = 0;
                    f[1153] = f[1262];
                    f[1152] = -1;
                    b = 0;
                    do {
                        w = 4616 + (b << 1 << 2) | 0;
                        f[w + 12 >> 2] = w;
                        f[w + 8 >> 2] = w;
                        b = b + 1 | 0;
                    } while ((b | 0) != 32);
                    w = g + 8 | 0;
                    w = (w & 7 | 0) == 0 ? 0 : 0 - w & 7;
                    v = g + w | 0;
                    w = h + -40 - w | 0;
                    f[1150] = v;
                    f[1147] = w;
                    f[v + 4 >> 2] = w | 1;
                    f[v + w + 4 >> 2] = 40;
                    f[1151] = f[1266];
                } while (0);
                b = f[1147] | 0;
                if (b >>> 0 > m >>> 0) {
                    t = b - m | 0;
                    f[1147] = t;
                    w = f[1150] | 0;
                    v = w + m | 0;
                    f[1150] = v;
                    f[v + 4 >> 2] = t | 1;
                    f[w + 4 >> 2] = m | 3;
                    w = w + 8 | 0;
                    u = x;
                    return w | 0
                }
            }
            w = jd() | 0;
            f[w >> 2] = 12;
            w = 0;
            u = x;
            return w | 0
        }

        function Na(a, c, d, e, g, i) {
            a = a | 0;
            c = +c;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            i = i | 0;
            var j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0.0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0,
                y = 0, z = 0, A = 0, B = 0, C = 0, D = 0, E = 0, F = 0, G = 0;
            G = u;
            u = u + 560 | 0;
            l = G + 8 | 0;
            t = G;
            F = G + 524 | 0;
            E = F;
            m = G + 512 | 0;
            f[t >> 2] = 0;
            D = m + 12 | 0;
            Fc(c) | 0;
            if ((I | 0) < 0) {
                c = -c;
                B = 1;
                A = 2087;
            } else {
                B = (g & 2049 | 0) != 0 & 1;
                A = (g & 2048 | 0) == 0 ? ((g & 1 | 0) == 0 ? 2088 : 2093) : 2090;
            }
            Fc(c) | 0;
            C = I & 2146435072;
            do if (C >>> 0 < 2146435072 | (C | 0) == 2146435072 & 0 < 0) {
                q = +hd(c, t) * 2.0;
                j = q != 0.0;
                if (j) f[t >> 2] = (f[t >> 2] | 0) + -1;
                w = i | 32;
                if ((w | 0) == 97) {
                    r = i & 32;
                    p = (r | 0) == 0 ? A : A + 9 | 0;
                    o = B | 2;
                    j = 12 - e | 0;
                    do if (!(e >>> 0 > 11 | (j | 0) == 0)) {
                        c = 8.0;
                        do {
                            j = j + -1 | 0;
                            c = c * 16.0;
                        } while ((j | 0) != 0);
                        if ((b[p >> 0] | 0) == 45) {
                            c = -(c + (-q - c));
                            break
                        } else {
                            c = q + c - c;
                            break
                        }
                    } else c = q; while (0);
                    k = f[t >> 2] | 0;
                    j = (k | 0) < 0 ? 0 - k | 0 : k;
                    j = Rb(j, ((j | 0) < 0) << 31 >> 31, D) | 0;
                    if ((j | 0) == (D | 0)) {
                        j = m + 11 | 0;
                        b[j >> 0] = 48;
                    }
                    b[j + -1 >> 0] = (k >> 31 & 2) + 43;
                    n = j + -2 | 0;
                    b[n >> 0] = i + 15;
                    m = (e | 0) < 1;
                    l = (g & 8 | 0) == 0;
                    j = F;
                    do {
                        C = ~~c;
                        k = j + 1 | 0;
                        b[j >> 0] = h[2122 + C >> 0] | r;
                        c = (c - +(C | 0)) * 16.0;
                        if ((k - E | 0) == 1 ? !(l & (m & c == 0.0)) : 0) {
                            b[k >> 0] = 46;
                            j = j + 2 | 0;
                        } else j = k;
                    } while (c != 0.0);
                    C = j - E | 0;
                    E = D - n | 0;
                    D = (e | 0) != 0 & (C + -2 | 0) < (e | 0) ? e + 2 | 0 : C;
                    j = E + o + D | 0;
                    Wb(a, 32, d, j, g);
                    Nc(a, p, o);
                    Wb(a, 48, d, j, g ^ 65536);
                    Nc(a, F, C);
                    Wb(a, 48, D - C | 0, 0, 0);
                    Nc(a, n, E);
                    Wb(a, 32, d, j, g ^ 8192);
                    break
                }
                k = (e | 0) < 0 ? 6 : e;
                if (j) {
                    j = (f[t >> 2] | 0) + -28 | 0;
                    f[t >> 2] = j;
                    c = q * 268435456.0;
                } else {
                    c = q;
                    j = f[t >> 2] | 0;
                }
                C = (j | 0) < 0 ? l : l + 288 | 0;
                l = C;
                do {
                    y = ~~c >>> 0;
                    f[l >> 2] = y;
                    l = l + 4 | 0;
                    c = (c - +(y >>> 0)) * 1.0e9;
                } while (c != 0.0);
                if ((j | 0) > 0) {
                    m = C;
                    o = l;
                    while (1) {
                        n = (j | 0) < 29 ? j : 29;
                        j = o + -4 | 0;
                        if (j >>> 0 >= m >>> 0) {
                            l = 0;
                            do {
                                x = yc(f[j >> 2] | 0, 0, n | 0) | 0;
                                x = Gc(x | 0, I | 0, l | 0, 0) | 0;
                                y = I;
                                v = mc(x | 0, y | 0, 1e9, 0) | 0;
                                f[j >> 2] = v;
                                l = Uc(x | 0, y | 0, 1e9, 0) | 0;
                                j = j + -4 | 0;
                            } while (j >>> 0 >= m >>> 0);
                            if (l) {
                                m = m + -4 | 0;
                                f[m >> 2] = l;
                            }
                        }
                        l = o;
                        while (1) {
                            if (l >>> 0 <= m >>> 0) break;
                            j = l + -4 | 0;
                            if (!(f[j >> 2] | 0)) l = j; else break
                        }
                        j = (f[t >> 2] | 0) - n | 0;
                        f[t >> 2] = j;
                        if ((j | 0) > 0) o = l; else break
                    }
                } else m = C;
                if ((j | 0) < 0) {
                    e = ((k + 25 | 0) / 9 | 0) + 1 | 0;
                    s = (w | 0) == 102;
                    do {
                        r = 0 - j | 0;
                        r = (r | 0) < 9 ? r : 9;
                        if (m >>> 0 < l >>> 0) {
                            n = (1 << r) + -1 | 0;
                            o = 1e9 >>> r;
                            p = 0;
                            j = m;
                            do {
                                y = f[j >> 2] | 0;
                                f[j >> 2] = (y >>> r) + p;
                                p = X(y & n, o) | 0;
                                j = j + 4 | 0;
                            } while (j >>> 0 < l >>> 0);
                            j = (f[m >> 2] | 0) == 0 ? m + 4 | 0 : m;
                            if (!p) {
                                m = j;
                                j = l;
                            } else {
                                f[l >> 2] = p;
                                m = j;
                                j = l + 4 | 0;
                            }
                        } else {
                            m = (f[m >> 2] | 0) == 0 ? m + 4 | 0 : m;
                            j = l;
                        }
                        l = s ? C : m;
                        l = (j - l >> 2 | 0) > (e | 0) ? l + (e << 2) | 0 : j;
                        j = (f[t >> 2] | 0) + r | 0;
                        f[t >> 2] = j;
                    } while ((j | 0) < 0);
                    j = m;
                    e = l;
                } else {
                    j = m;
                    e = l;
                }
                y = C;
                if (j >>> 0 < e >>> 0) {
                    l = (y - j >> 2) * 9 | 0;
                    n = f[j >> 2] | 0;
                    if (n >>> 0 >= 10) {
                        m = 10;
                        do {
                            m = m * 10 | 0;
                            l = l + 1 | 0;
                        } while (n >>> 0 >= m >>> 0)
                    }
                } else l = 0;
                s = (w | 0) == 103;
                v = (k | 0) != 0;
                m = k - ((w | 0) != 102 ? l : 0) + ((v & s) << 31 >> 31) | 0;
                if ((m | 0) < (((e - y >> 2) * 9 | 0) + -9 | 0)) {
                    m = m + 9216 | 0;
                    r = C + 4 + (((m | 0) / 9 | 0) + -1024 << 2) | 0;
                    m = ((m | 0) % 9 | 0) + 1 | 0;
                    if ((m | 0) < 9) {
                        n = 10;
                        do {
                            n = n * 10 | 0;
                            m = m + 1 | 0;
                        } while ((m | 0) != 9)
                    } else n = 10;
                    o = f[r >> 2] | 0;
                    p = (o >>> 0) % (n >>> 0) | 0;
                    m = (r + 4 | 0) == (e | 0);
                    if (!(m & (p | 0) == 0)) {
                        q = (((o >>> 0) / (n >>> 0) | 0) & 1 | 0) == 0 ? 9007199254740992.0 : 9007199254740994.0;
                        x = (n | 0) / 2 | 0;
                        c = p >>> 0 < x >>> 0 ? .5 : m & (p | 0) == (x | 0) ? 1.0 : 1.5;
                        if (B) {
                            x = (b[A >> 0] | 0) == 45;
                            c = x ? -c : c;
                            q = x ? -q : q;
                        }
                        m = o - p | 0;
                        f[r >> 2] = m;
                        if (q + c != q) {
                            x = m + n | 0;
                            f[r >> 2] = x;
                            if (x >>> 0 > 999999999) {
                                l = r;
                                while (1) {
                                    m = l + -4 | 0;
                                    f[l >> 2] = 0;
                                    if (m >>> 0 < j >>> 0) {
                                        j = j + -4 | 0;
                                        f[j >> 2] = 0;
                                    }
                                    x = (f[m >> 2] | 0) + 1 | 0;
                                    f[m >> 2] = x;
                                    if (x >>> 0 > 999999999) l = m; else break
                                }
                            } else m = r;
                            l = (y - j >> 2) * 9 | 0;
                            o = f[j >> 2] | 0;
                            if (o >>> 0 >= 10) {
                                n = 10;
                                do {
                                    n = n * 10 | 0;
                                    l = l + 1 | 0;
                                } while (o >>> 0 >= n >>> 0)
                            }
                        } else m = r;
                    } else m = r;
                    m = m + 4 | 0;
                    m = e >>> 0 > m >>> 0 ? m : e;
                    x = j;
                } else {
                    m = e;
                    x = j;
                }
                w = m;
                while (1) {
                    if (w >>> 0 <= x >>> 0) {
                        t = 0;
                        break
                    }
                    j = w + -4 | 0;
                    if (!(f[j >> 2] | 0)) w = j; else {
                        t = 1;
                        break
                    }
                }
                e = 0 - l | 0;
                do if (s) {
                    j = ((v ^ 1) & 1) + k | 0;
                    if ((j | 0) > (l | 0) & (l | 0) > -5) {
                        n = i + -1 | 0;
                        k = j + -1 - l | 0;
                    } else {
                        n = i + -2 | 0;
                        k = j + -1 | 0;
                    }
                    j = g & 8;
                    if (!j) {
                        if (t ? (z = f[w + -4 >> 2] | 0, (z | 0) != 0) : 0) if (!((z >>> 0) % 10 | 0)) {
                            m = 0;
                            j = 10;
                            do {
                                j = j * 10 | 0;
                                m = m + 1 | 0;
                            } while (!((z >>> 0) % (j >>> 0) | 0 | 0))
                        } else m = 0; else m = 9;
                        j = ((w - y >> 2) * 9 | 0) + -9 | 0;
                        if ((n | 32 | 0) == 102) {
                            r = j - m | 0;
                            r = (r | 0) > 0 ? r : 0;
                            k = (k | 0) < (r | 0) ? k : r;
                            r = 0;
                            break
                        } else {
                            r = j + l - m | 0;
                            r = (r | 0) > 0 ? r : 0;
                            k = (k | 0) < (r | 0) ? k : r;
                            r = 0;
                            break
                        }
                    } else r = j;
                } else {
                    n = i;
                    r = g & 8;
                } while (0);
                s = k | r;
                o = (s | 0) != 0 & 1;
                p = (n | 32 | 0) == 102;
                if (p) {
                    v = 0;
                    j = (l | 0) > 0 ? l : 0;
                } else {
                    j = (l | 0) < 0 ? e : l;
                    j = Rb(j, ((j | 0) < 0) << 31 >> 31, D) | 0;
                    m = D;
                    if ((m - j | 0) < 2) do {
                        j = j + -1 | 0;
                        b[j >> 0] = 48;
                    } while ((m - j | 0) < 2);
                    b[j + -1 >> 0] = (l >> 31 & 2) + 43;
                    j = j + -2 | 0;
                    b[j >> 0] = n;
                    v = j;
                    j = m - j | 0;
                }
                j = B + 1 + k + o + j | 0;
                Wb(a, 32, d, j, g);
                Nc(a, A, B);
                Wb(a, 48, d, j, g ^ 65536);
                if (p) {
                    n = x >>> 0 > C >>> 0 ? C : x;
                    r = F + 9 | 0;
                    o = r;
                    p = F + 8 | 0;
                    m = n;
                    do {
                        l = Rb(f[m >> 2] | 0, 0, r) | 0;
                        if ((m | 0) == (n | 0)) {
                            if ((l | 0) == (r | 0)) {
                                b[p >> 0] = 48;
                                l = p;
                            }
                        } else if (l >>> 0 > F >>> 0) {
                            Ib(F | 0, 48, l - E | 0) | 0;
                            do l = l + -1 | 0; while (l >>> 0 > F >>> 0)
                        }
                        Nc(a, l, o - l | 0);
                        m = m + 4 | 0;
                    } while (m >>> 0 <= C >>> 0);
                    if (s | 0) Nc(a, 2138, 1);
                    if (m >>> 0 < w >>> 0 & (k | 0) > 0) while (1) {
                        l = Rb(f[m >> 2] | 0, 0, r) | 0;
                        if (l >>> 0 > F >>> 0) {
                            Ib(F | 0, 48, l - E | 0) | 0;
                            do l = l + -1 | 0; while (l >>> 0 > F >>> 0)
                        }
                        Nc(a, l, (k | 0) < 9 ? k : 9);
                        m = m + 4 | 0;
                        l = k + -9 | 0;
                        if (!(m >>> 0 < w >>> 0 & (k | 0) > 9)) {
                            k = l;
                            break
                        } else k = l;
                    }
                    Wb(a, 48, k + 9 | 0, 9, 0);
                } else {
                    s = t ? w : x + 4 | 0;
                    if ((k | 0) > -1) {
                        t = F + 9 | 0;
                        r = (r | 0) == 0;
                        e = t;
                        o = 0 - E | 0;
                        p = F + 8 | 0;
                        n = x;
                        do {
                            l = Rb(f[n >> 2] | 0, 0, t) | 0;
                            if ((l | 0) == (t | 0)) {
                                b[p >> 0] = 48;
                                l = p;
                            }
                            do if ((n | 0) == (x | 0)) {
                                m = l + 1 | 0;
                                Nc(a, l, 1);
                                if (r & (k | 0) < 1) {
                                    l = m;
                                    break
                                }
                                Nc(a, 2138, 1);
                                l = m;
                            } else {
                                if (l >>> 0 <= F >>> 0) break;
                                Ib(F | 0, 48, l + o | 0) | 0;
                                do l = l + -1 | 0; while (l >>> 0 > F >>> 0)
                            } while (0);
                            E = e - l | 0;
                            Nc(a, l, (k | 0) > (E | 0) ? E : k);
                            k = k - E | 0;
                            n = n + 4 | 0;
                        } while (n >>> 0 < s >>> 0 & (k | 0) > -1)
                    }
                    Wb(a, 48, k + 18 | 0, 18, 0);
                    Nc(a, v, D - v | 0);
                }
                Wb(a, 32, d, j, g ^ 8192);
            } else {
                F = (i & 32 | 0) != 0;
                j = B + 3 | 0;
                Wb(a, 32, d, j, g & -65537);
                Nc(a, A, B);
                Nc(a, c != c | 0.0 != 0.0 ? (F ? 2114 : 2118) : F ? 2106 : 2110, 3);
                Wb(a, 32, d, j, g ^ 8192);
            } while (0);
            u = G;
            return ((j | 0) < (d | 0) ? d : j) | 0
        }

        function Oa(a, c, e, g, h) {
            a = a | 0;
            c = c | 0;
            e = e | 0;
            g = g | 0;
            h = h | 0;
            var i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, q = 0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0, y = 0,
                z = 0, A = 0, B = 0, C = 0, D = 0, E = 0, F = 0, G = 0;
            G = u;
            u = u + 64 | 0;
            C = G + 16 | 0;
            D = G;
            A = G + 24 | 0;
            E = G + 8 | 0;
            F = G + 20 | 0;
            f[C >> 2] = c;
            x = (a | 0) != 0;
            y = A + 40 | 0;
            z = y;
            A = A + 39 | 0;
            B = E + 4 | 0;
            j = 0;
            i = 0;
            n = 0;
            a:while (1) {
                do if ((i | 0) > -1) if ((j | 0) > (2147483647 - i | 0)) {
                    i = jd() | 0;
                    f[i >> 2] = 75;
                    i = -1;
                    break
                } else {
                    i = j + i | 0;
                    break
                } while (0);
                j = b[c >> 0] | 0;
                if (!(j << 24 >> 24)) {
                    w = 87;
                    break
                } else k = c;
                b:while (1) {
                    switch (j << 24 >> 24) {
                        case 37: {
                            j = k;
                            w = 9;
                            break b
                        }
                        case 0: {
                            j = k;
                            break b
                        }
                    }
                    v = k + 1 | 0;
                    f[C >> 2] = v;
                    j = b[v >> 0] | 0;
                    k = v;
                }
                c:do if ((w | 0) == 9) while (1) {
                    w = 0;
                    if ((b[k + 1 >> 0] | 0) != 37) break c;
                    j = j + 1 | 0;
                    k = k + 2 | 0;
                    f[C >> 2] = k;
                    if ((b[k >> 0] | 0) == 37) w = 9; else break
                } while (0);
                j = j - c | 0;
                if (x) Nc(a, c, j);
                if (j | 0) {
                    c = k;
                    continue
                }
                l = k + 1 | 0;
                j = (b[l >> 0] | 0) + -48 | 0;
                if (j >>> 0 < 10) {
                    v = (b[k + 2 >> 0] | 0) == 36;
                    t = v ? j : -1;
                    n = v ? 1 : n;
                    l = v ? k + 3 | 0 : l;
                } else t = -1;
                f[C >> 2] = l;
                j = b[l >> 0] | 0;
                k = (j << 24 >> 24) + -32 | 0;
                d:do if (k >>> 0 < 32) {
                    m = 0;
                    o = j;
                    while (1) {
                        j = 1 << k;
                        if (!(j & 75913)) {
                            j = o;
                            break d
                        }
                        m = j | m;
                        l = l + 1 | 0;
                        f[C >> 2] = l;
                        j = b[l >> 0] | 0;
                        k = (j << 24 >> 24) + -32 | 0;
                        if (k >>> 0 >= 32) break; else o = j;
                    }
                } else m = 0; while (0);
                if (j << 24 >> 24 == 42) {
                    k = l + 1 | 0;
                    j = (b[k >> 0] | 0) + -48 | 0;
                    if (j >>> 0 < 10 ? (b[l + 2 >> 0] | 0) == 36 : 0) {
                        f[h + (j << 2) >> 2] = 10;
                        j = f[g + ((b[k >> 0] | 0) + -48 << 3) >> 2] | 0;
                        n = 1;
                        l = l + 3 | 0;
                    } else {
                        if (n | 0) {
                            i = -1;
                            break
                        }
                        if (x) {
                            n = (f[e >> 2] | 0) + (4 - 1) & ~(4 - 1);
                            j = f[n >> 2] | 0;
                            f[e >> 2] = n + 4;
                            n = 0;
                            l = k;
                        } else {
                            j = 0;
                            n = 0;
                            l = k;
                        }
                    }
                    f[C >> 2] = l;
                    v = (j | 0) < 0;
                    j = v ? 0 - j | 0 : j;
                    m = v ? m | 8192 : m;
                } else {
                    j = ec(C) | 0;
                    if ((j | 0) < 0) {
                        i = -1;
                        break
                    }
                    l = f[C >> 2] | 0;
                }
                do if ((b[l >> 0] | 0) == 46) {
                    if ((b[l + 1 >> 0] | 0) != 42) {
                        f[C >> 2] = l + 1;
                        k = ec(C) | 0;
                        l = f[C >> 2] | 0;
                        break
                    }
                    o = l + 2 | 0;
                    k = (b[o >> 0] | 0) + -48 | 0;
                    if (k >>> 0 < 10 ? (b[l + 3 >> 0] | 0) == 36 : 0) {
                        f[h + (k << 2) >> 2] = 10;
                        k = f[g + ((b[o >> 0] | 0) + -48 << 3) >> 2] | 0;
                        l = l + 4 | 0;
                        f[C >> 2] = l;
                        break
                    }
                    if (n | 0) {
                        i = -1;
                        break a
                    }
                    if (x) {
                        v = (f[e >> 2] | 0) + (4 - 1) & ~(4 - 1);
                        k = f[v >> 2] | 0;
                        f[e >> 2] = v + 4;
                    } else k = 0;
                    f[C >> 2] = o;
                    l = o;
                } else k = -1; while (0);
                s = 0;
                while (1) {
                    if (((b[l >> 0] | 0) + -65 | 0) >>> 0 > 57) {
                        i = -1;
                        break a
                    }
                    v = l + 1 | 0;
                    f[C >> 2] = v;
                    o = b[(b[l >> 0] | 0) + -65 + (1606 + (s * 58 | 0)) >> 0] | 0;
                    q = o & 255;
                    if ((q + -1 | 0) >>> 0 < 8) {
                        s = q;
                        l = v;
                    } else break
                }
                if (!(o << 24 >> 24)) {
                    i = -1;
                    break
                }
                r = (t | 0) > -1;
                do if (o << 24 >> 24 == 19) if (r) {
                    i = -1;
                    break a
                } else w = 49; else {
                    if (r) {
                        f[h + (t << 2) >> 2] = q;
                        r = g + (t << 3) | 0;
                        t = f[r + 4 >> 2] | 0;
                        w = D;
                        f[w >> 2] = f[r >> 2];
                        f[w + 4 >> 2] = t;
                        w = 49;
                        break
                    }
                    if (!x) {
                        i = 0;
                        break a
                    }
                    db(D, q, e);
                } while (0);
                if ((w | 0) == 49 ? (w = 0, !x) : 0) {
                    j = 0;
                    c = v;
                    continue
                }
                l = b[l >> 0] | 0;
                l = (s | 0) != 0 & (l & 15 | 0) == 3 ? l & -33 : l;
                r = m & -65537;
                t = (m & 8192 | 0) == 0 ? m : r;
                e:do switch (l | 0) {
                    case 110:
                        switch ((s & 255) << 24 >> 24) {
                            case 0: {
                                f[f[D >> 2] >> 2] = i;
                                j = 0;
                                c = v;
                                continue a
                            }
                            case 1: {
                                f[f[D >> 2] >> 2] = i;
                                j = 0;
                                c = v;
                                continue a
                            }
                            case 2: {
                                j = f[D >> 2] | 0;
                                f[j >> 2] = i;
                                f[j + 4 >> 2] = ((i | 0) < 0) << 31 >> 31;
                                j = 0;
                                c = v;
                                continue a
                            }
                            case 3: {
                                d[f[D >> 2] >> 1] = i;
                                j = 0;
                                c = v;
                                continue a
                            }
                            case 4: {
                                b[f[D >> 2] >> 0] = i;
                                j = 0;
                                c = v;
                                continue a
                            }
                            case 6: {
                                f[f[D >> 2] >> 2] = i;
                                j = 0;
                                c = v;
                                continue a
                            }
                            case 7: {
                                j = f[D >> 2] | 0;
                                f[j >> 2] = i;
                                f[j + 4 >> 2] = ((i | 0) < 0) << 31 >> 31;
                                j = 0;
                                c = v;
                                continue a
                            }
                            default: {
                                j = 0;
                                c = v;
                                continue a
                            }
                        }
                    case 112: {
                        l = 120;
                        k = k >>> 0 > 8 ? k : 8;
                        c = t | 8;
                        w = 61;
                        break
                    }
                    case 88:
                    case 120: {
                        c = t;
                        w = 61;
                        break
                    }
                    case 111: {
                        l = D;
                        c = f[l >> 2] | 0;
                        l = f[l + 4 >> 2] | 0;
                        q = kc(c, l, y) | 0;
                        r = z - q | 0;
                        m = 0;
                        o = 2070;
                        k = (t & 8 | 0) == 0 | (k | 0) > (r | 0) ? k : r + 1 | 0;
                        r = t;
                        w = 67;
                        break
                    }
                    case 105:
                    case 100: {
                        l = D;
                        c = f[l >> 2] | 0;
                        l = f[l + 4 >> 2] | 0;
                        if ((l | 0) < 0) {
                            c = Cc(0, 0, c | 0, l | 0) | 0;
                            l = I;
                            m = D;
                            f[m >> 2] = c;
                            f[m + 4 >> 2] = l;
                            m = 1;
                            o = 2070;
                            w = 66;
                            break e
                        } else {
                            m = (t & 2049 | 0) != 0 & 1;
                            o = (t & 2048 | 0) == 0 ? ((t & 1 | 0) == 0 ? 2070 : 2072) : 2071;
                            w = 66;
                            break e
                        }
                    }
                    case 117: {
                        l = D;
                        m = 0;
                        o = 2070;
                        c = f[l >> 2] | 0;
                        l = f[l + 4 >> 2] | 0;
                        w = 66;
                        break
                    }
                    case 99: {
                        b[A >> 0] = f[D >> 2];
                        c = A;
                        m = 0;
                        o = 2070;
                        q = y;
                        l = 1;
                        k = r;
                        break
                    }
                    case 109: {
                        l = jd() | 0;
                        l = Qc(f[l >> 2] | 0) | 0;
                        w = 71;
                        break
                    }
                    case 115: {
                        l = f[D >> 2] | 0;
                        l = l | 0 ? l : 2080;
                        w = 71;
                        break
                    }
                    case 67: {
                        f[E >> 2] = f[D >> 2];
                        f[B >> 2] = 0;
                        f[D >> 2] = E;
                        q = -1;
                        l = E;
                        w = 75;
                        break
                    }
                    case 83: {
                        c = f[D >> 2] | 0;
                        if (!k) {
                            Wb(a, 32, j, 0, t);
                            c = 0;
                            w = 84;
                        } else {
                            q = k;
                            l = c;
                            w = 75;
                        }
                        break
                    }
                    case 65:
                    case 71:
                    case 70:
                    case 69:
                    case 97:
                    case 103:
                    case 102:
                    case 101: {
                        j = Na(a, +p[D >> 3], j, k, t, l) | 0;
                        c = v;
                        continue a
                    }
                    default: {
                        m = 0;
                        o = 2070;
                        q = y;
                        l = k;
                        k = t;
                    }
                } while (0);
                f:do if ((w | 0) == 61) {
                    t = D;
                    s = f[t >> 2] | 0;
                    t = f[t + 4 >> 2] | 0;
                    q = gc(s, t, y, l & 32) | 0;
                    o = (c & 8 | 0) == 0 | (s | 0) == 0 & (t | 0) == 0;
                    m = o ? 0 : 2;
                    o = o ? 2070 : 2070 + (l >> 4) | 0;
                    r = c;
                    c = s;
                    l = t;
                    w = 67;
                } else if ((w | 0) == 66) {
                    q = Rb(c, l, y) | 0;
                    r = t;
                    w = 67;
                } else if ((w | 0) == 71) {
                    w = 0;
                    t = vb(l, 0, k) | 0;
                    s = (t | 0) == 0;
                    c = l;
                    m = 0;
                    o = 2070;
                    q = s ? l + k | 0 : t;
                    l = s ? k : t - l | 0;
                    k = r;
                } else if ((w | 0) == 75) {
                    w = 0;
                    o = l;
                    c = 0;
                    k = 0;
                    while (1) {
                        m = f[o >> 2] | 0;
                        if (!m) break;
                        k = Rc(F, m) | 0;
                        if ((k | 0) < 0 | k >>> 0 > (q - c | 0) >>> 0) break;
                        c = k + c | 0;
                        if (q >>> 0 > c >>> 0) o = o + 4 | 0; else break
                    }
                    if ((k | 0) < 0) {
                        i = -1;
                        break a
                    }
                    Wb(a, 32, j, c, t);
                    if (!c) {
                        c = 0;
                        w = 84;
                    } else {
                        m = 0;
                        while (1) {
                            k = f[l >> 2] | 0;
                            if (!k) {
                                w = 84;
                                break f
                            }
                            k = Rc(F, k) | 0;
                            m = k + m | 0;
                            if ((m | 0) > (c | 0)) {
                                w = 84;
                                break f
                            }
                            Nc(a, F, k);
                            if (m >>> 0 >= c >>> 0) {
                                w = 84;
                                break
                            } else l = l + 4 | 0;
                        }
                    }
                } while (0);
                if ((w | 0) == 67) {
                    w = 0;
                    l = (c | 0) != 0 | (l | 0) != 0;
                    t = (k | 0) != 0 | l;
                    l = ((l ^ 1) & 1) + (z - q) | 0;
                    c = t ? q : y;
                    q = y;
                    l = t ? ((k | 0) > (l | 0) ? k : l) : k;
                    k = (k | 0) > -1 ? r & -65537 : r;
                } else if ((w | 0) == 84) {
                    w = 0;
                    Wb(a, 32, j, c, t ^ 8192);
                    j = (j | 0) > (c | 0) ? j : c;
                    c = v;
                    continue
                }
                s = q - c | 0;
                r = (l | 0) < (s | 0) ? s : l;
                t = r + m | 0;
                j = (j | 0) < (t | 0) ? t : j;
                Wb(a, 32, j, t, k);
                Nc(a, o, m);
                Wb(a, 48, j, t, k ^ 65536);
                Wb(a, 48, r, s, 0);
                Nc(a, c, s);
                Wb(a, 32, j, t, k ^ 8192);
                c = v;
            }
            g:do if ((w | 0) == 87) if (!a) if (!n) i = 0; else {
                i = 1;
                while (1) {
                    c = f[h + (i << 2) >> 2] | 0;
                    if (!c) break;
                    db(g + (i << 3) | 0, c, e);
                    i = i + 1 | 0;
                    if ((i | 0) >= 10) {
                        i = 1;
                        break g
                    }
                }
                while (1) {
                    if (f[h + (i << 2) >> 2] | 0) {
                        i = -1;
                        break g
                    }
                    i = i + 1 | 0;
                    if ((i | 0) >= 10) {
                        i = 1;
                        break
                    }
                }
            } while (0);
            u = G;
            return i | 0
        }

        function Pa(a, c) {
            a = a | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0,
                v = 0, w = 0, x = 0, y = 0, z = 0, A = 0, B = 0, C = 0, D = 0, E = 0;
            E = u;
            u = u + 704 | 0;
            A = E + 144 | 0;
            z = E + 128 | 0;
            y = E + 112 | 0;
            x = E + 96 | 0;
            w = E + 80 | 0;
            v = E + 64 | 0;
            t = E + 48 | 0;
            B = E + 32 | 0;
            n = E + 16 | 0;
            k = E;
            p = E + 184 | 0;
            D = E + 160 | 0;
            q = yb(a, 14) | 0;
            if (!q) {
                Eb(c);
                D = 1;
                u = E;
                return D | 0
            }
            r = c + 4 | 0;
            s = c + 8 | 0;
            d = f[s >> 2] | 0;
            if ((d | 0) != (q | 0)) {
                if (d >>> 0 <= q >>> 0) {
                    do if ((f[c + 12 >> 2] | 0) >>> 0 < q >>> 0) {
                        if (fb(r, q, (d + 1 | 0) == (q | 0), 1, 0) | 0) {
                            d = f[s >> 2] | 0;
                            break
                        }
                        b[c + 16 >> 0] = 1;
                        D = 0;
                        u = E;
                        return D | 0
                    } while (0);
                    Ib((f[r >> 2] | 0) + d | 0, 0, q - d | 0) | 0;
                }
                f[s >> 2] = q;
            }
            Ib(f[r >> 2] | 0, 0, q | 0) | 0;
            o = a + 20 | 0;
            d = f[o >> 2] | 0;
            if ((d | 0) < 5) {
                i = a + 4 | 0;
                j = a + 8 | 0;
                g = a + 16 | 0;
                do {
                    e = f[i >> 2] | 0;
                    if ((e | 0) == (f[j >> 2] | 0)) e = 0; else {
                        f[i >> 2] = e + 1;
                        e = h[e >> 0] | 0;
                    }
                    d = d + 8 | 0;
                    f[o >> 2] = d;
                    if ((d | 0) >= 33) {
                        f[k >> 2] = 866;
                        f[k + 4 >> 2] = 3208;
                        f[k + 8 >> 2] = 1366;
                        vc(p, 812, k) | 0;
                        Ub(p) | 0;
                        d = f[o >> 2] | 0;
                    }
                    e = e << 32 - d | f[g >> 2];
                    f[g >> 2] = e;
                } while ((d | 0) < 5)
            } else {
                e = a + 16 | 0;
                g = e;
                e = f[e >> 2] | 0;
            }
            m = e >>> 27;
            f[g >> 2] = e << 5;
            f[o >> 2] = d + -5;
            if ((m + -1 | 0) >>> 0 > 20) {
                D = 0;
                u = E;
                return D | 0
            }
            f[D + 20 >> 2] = 0;
            f[D >> 2] = 0;
            f[D + 4 >> 2] = 0;
            f[D + 8 >> 2] = 0;
            f[D + 12 >> 2] = 0;
            b[D + 16 >> 0] = 0;
            d = D + 4 | 0;
            e = D + 8 | 0;
            a:do if (fb(d, 21, 0, 1, 0) | 0) {
                i = f[e >> 2] | 0;
                l = f[d >> 2] | 0;
                Ib(l + i | 0, 0, 21 - i | 0) | 0;
                f[e >> 2] = 21;
                i = a + 4 | 0;
                j = a + 8 | 0;
                k = a + 16 | 0;
                g = 0;
                do {
                    d = f[o >> 2] | 0;
                    if ((d | 0) < 3) do {
                        e = f[i >> 2] | 0;
                        if ((e | 0) == (f[j >> 2] | 0)) e = 0; else {
                            f[i >> 2] = e + 1;
                            e = h[e >> 0] | 0;
                        }
                        d = d + 8 | 0;
                        f[o >> 2] = d;
                        if ((d | 0) >= 33) {
                            f[n >> 2] = 866;
                            f[n + 4 >> 2] = 3208;
                            f[n + 8 >> 2] = 1366;
                            vc(p, 812, n) | 0;
                            Ub(p) | 0;
                            d = f[o >> 2] | 0;
                        }
                        e = e << 32 - d | f[k >> 2];
                        f[k >> 2] = e;
                    } while ((d | 0) < 3); else e = f[k >> 2] | 0;
                    f[k >> 2] = e << 3;
                    f[o >> 2] = d + -3;
                    b[l + (h[1327 + g >> 0] | 0) >> 0] = e >>> 29;
                    g = g + 1 | 0;
                } while ((g | 0) != (m | 0));
                if (qb(D) | 0) {
                    k = a + 4 | 0;
                    l = a + 8 | 0;
                    m = a + 16 | 0;
                    d = 0;
                    b:do {
                        j = q - d | 0;
                        g = bb(a, D) | 0;
                        c:do if (g >>> 0 < 17) {
                            if ((f[s >> 2] | 0) >>> 0 <= d >>> 0) {
                                f[B >> 2] = 866;
                                f[B + 4 >> 2] = 910;
                                f[B + 8 >> 2] = 1497;
                                vc(p, 812, B) | 0;
                                Ub(p) | 0;
                            }
                            b[(f[r >> 2] | 0) + d >> 0] = g;
                            d = d + 1 | 0;
                        } else switch (g | 0) {
                            case 17: {
                                e = f[o >> 2] | 0;
                                if ((e | 0) < 3) do {
                                    g = f[k >> 2] | 0;
                                    if ((g | 0) == (f[l >> 2] | 0)) g = 0; else {
                                        f[k >> 2] = g + 1;
                                        g = h[g >> 0] | 0;
                                    }
                                    e = e + 8 | 0;
                                    f[o >> 2] = e;
                                    if ((e | 0) >= 33) {
                                        f[t >> 2] = 866;
                                        f[t + 4 >> 2] = 3208;
                                        f[t + 8 >> 2] = 1366;
                                        vc(p, 812, t) | 0;
                                        Ub(p) | 0;
                                        e = f[o >> 2] | 0;
                                    }
                                    g = g << 32 - e | f[m >> 2];
                                    f[m >> 2] = g;
                                } while ((e | 0) < 3); else g = f[m >> 2] | 0;
                                f[m >> 2] = g << 3;
                                f[o >> 2] = e + -3;
                                g = (g >>> 29) + 3 | 0;
                                e = g >>> 0 > j >>> 0;
                                if (e) {
                                    d = 0;
                                    break a
                                } else {
                                    d = (e ? 0 : g) + d | 0;
                                    break c
                                }
                            }
                            case 18: {
                                e = f[o >> 2] | 0;
                                if ((e | 0) < 7) do {
                                    g = f[k >> 2] | 0;
                                    if ((g | 0) == (f[l >> 2] | 0)) g = 0; else {
                                        f[k >> 2] = g + 1;
                                        g = h[g >> 0] | 0;
                                    }
                                    e = e + 8 | 0;
                                    f[o >> 2] = e;
                                    if ((e | 0) >= 33) {
                                        f[v >> 2] = 866;
                                        f[v + 4 >> 2] = 3208;
                                        f[v + 8 >> 2] = 1366;
                                        vc(p, 812, v) | 0;
                                        Ub(p) | 0;
                                        e = f[o >> 2] | 0;
                                    }
                                    g = g << 32 - e | f[m >> 2];
                                    f[m >> 2] = g;
                                } while ((e | 0) < 7); else g = f[m >> 2] | 0;
                                f[m >> 2] = g << 7;
                                f[o >> 2] = e + -7;
                                g = (g >>> 25) + 11 | 0;
                                e = g >>> 0 > j >>> 0;
                                if (e) {
                                    d = 0;
                                    break a
                                } else {
                                    d = (e ? 0 : g) + d | 0;
                                    break c
                                }
                            }
                            default: {
                                if ((g + -19 | 0) >>> 0 >= 2) {
                                    C = 81;
                                    break b
                                }
                                e = f[o >> 2] | 0;
                                if ((g | 0) == 19) {
                                    if ((e | 0) < 2) {
                                        g = e;
                                        while (1) {
                                            e = f[k >> 2] | 0;
                                            if ((e | 0) == (f[l >> 2] | 0)) i = 0; else {
                                                f[k >> 2] = e + 1;
                                                i = h[e >> 0] | 0;
                                            }
                                            e = g + 8 | 0;
                                            f[o >> 2] = e;
                                            if ((e | 0) >= 33) {
                                                f[w >> 2] = 866;
                                                f[w + 4 >> 2] = 3208;
                                                f[w + 8 >> 2] = 1366;
                                                vc(p, 812, w) | 0;
                                                Ub(p) | 0;
                                                e = f[o >> 2] | 0;
                                            }
                                            g = i << 32 - e | f[m >> 2];
                                            f[m >> 2] = g;
                                            if ((e | 0) < 2) g = e; else break
                                        }
                                    } else g = f[m >> 2] | 0;
                                    f[m >> 2] = g << 2;
                                    g = g >>> 30;
                                    i = 3;
                                    e = e + -2 | 0;
                                } else {
                                    if ((e | 0) < 6) do {
                                        g = f[k >> 2] | 0;
                                        if ((g | 0) == (f[l >> 2] | 0)) g = 0; else {
                                            f[k >> 2] = g + 1;
                                            g = h[g >> 0] | 0;
                                        }
                                        e = e + 8 | 0;
                                        f[o >> 2] = e;
                                        if ((e | 0) >= 33) {
                                            f[x >> 2] = 866;
                                            f[x + 4 >> 2] = 3208;
                                            f[x + 8 >> 2] = 1366;
                                            vc(p, 812, x) | 0;
                                            Ub(p) | 0;
                                            e = f[o >> 2] | 0;
                                        }
                                        g = g << 32 - e | f[m >> 2];
                                        f[m >> 2] = g;
                                    } while ((e | 0) < 6); else g = f[m >> 2] | 0;
                                    f[m >> 2] = g << 6;
                                    g = g >>> 26;
                                    i = 7;
                                    e = e + -6 | 0;
                                }
                                f[o >> 2] = e;
                                g = g + i | 0;
                                if ((d | 0) == 0 | g >>> 0 > j >>> 0) {
                                    d = 0;
                                    break a
                                }
                                e = d + -1 | 0;
                                if ((f[s >> 2] | 0) >>> 0 <= e >>> 0) {
                                    f[y >> 2] = 866;
                                    f[y + 4 >> 2] = 910;
                                    f[y + 8 >> 2] = 1497;
                                    vc(p, 812, y) | 0;
                                    Ub(p) | 0;
                                }
                                i = b[(f[r >> 2] | 0) + e >> 0] | 0;
                                if (!(i << 24 >> 24)) {
                                    d = 0;
                                    break a
                                }
                                e = g + d | 0;
                                if (d >>> 0 >= e >>> 0) break c;
                                do {
                                    if ((f[s >> 2] | 0) >>> 0 <= d >>> 0) {
                                        f[z >> 2] = 866;
                                        f[z + 4 >> 2] = 910;
                                        f[z + 8 >> 2] = 1497;
                                        vc(p, 812, z) | 0;
                                        Ub(p) | 0;
                                    }
                                    b[(f[r >> 2] | 0) + d >> 0] = i;
                                    d = d + 1 | 0;
                                } while ((d | 0) != (e | 0));
                                d = e;
                            }
                        } while (0)
                    } while (q >>> 0 > d >>> 0);
                    if ((C | 0) == 81) {
                        f[A >> 2] = 866;
                        f[A + 4 >> 2] = 3149;
                        f[A + 8 >> 2] = 1348;
                        vc(p, 812, A) | 0;
                        Ub(p) | 0;
                        d = 0;
                        break
                    }
                    if ((q | 0) == (d | 0)) d = qb(c) | 0; else d = 0;
                } else d = 0;
            } else {
                b[D + 16 >> 0] = 1;
                d = 0;
            } while (0);
            Cb(D);
            D = d;
            u = E;
            return D | 0
        }

        function Qa(a, c, e, g) {
            a = a | 0;
            c = c | 0;
            e = e | 0;
            g = g | 0;
            var i = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0, y = 0,
                z = 0, A = 0, B = 0, C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0, J = 0, K = 0, L = 0;
            L = u;
            u = u + 880 | 0;
            J = L + 144 | 0;
            I = L + 128 | 0;
            H = L + 112 | 0;
            G = L + 96 | 0;
            C = L + 80 | 0;
            x = L + 64 | 0;
            v = L + 48 | 0;
            w = L + 32 | 0;
            q = L + 16 | 0;
            p = L;
            E = L + 360 | 0;
            F = L + 296 | 0;
            K = L + 224 | 0;
            t = L + 156 | 0;
            if ((c | 0) == 0 | g >>> 0 > 11) {
                K = 0;
                u = L;
                return K | 0
            }
            f[a >> 2] = c;
            i = K;
            k = i + 68 | 0;
            do {
                f[i >> 2] = 0;
                i = i + 4 | 0;
            } while ((i | 0) < (k | 0));
            i = 0;
            do {
                D = b[e + i >> 0] | 0;
                k = K + ((D & 255) << 2) | 0;
                if (D << 24 >> 24) f[k >> 2] = (f[k >> 2] | 0) + 1;
                i = i + 1 | 0;
            } while ((i | 0) != (c | 0));
            k = 0;
            l = 0;
            m = 0;
            n = -1;
            o = 1;
            while (1) {
                i = f[K + (o << 2) >> 2] | 0;
                if (!i) {
                    f[a + 28 + (o + -1 << 2) >> 2] = 0;
                    r = l;
                } else {
                    r = o + -1 | 0;
                    f[F + (r << 2) >> 2] = k;
                    k = i + k | 0;
                    D = 16 - o | 0;
                    f[a + 28 + (r << 2) >> 2] = (k + -1 << D | (1 << D) + -1) + 1;
                    f[a + 96 + (r << 2) >> 2] = l;
                    f[t + (o << 2) >> 2] = l;
                    r = i + l | 0;
                    m = m >>> 0 > o >>> 0 ? m : o;
                    n = n >>> 0 < o >>> 0 ? n : o;
                }
                o = o + 1 | 0;
                if ((o | 0) == 17) break; else {
                    k = k << 1;
                    l = r;
                }
            }
            f[a + 4 >> 2] = r;
            k = a + 172 | 0;
            do if (r >>> 0 > (f[k >> 2] | 0) >>> 0) {
                i = r + -1 | 0;
                if (!(i & r)) i = r; else {
                    i = i >>> 16 | i;
                    i = i >>> 8 | i;
                    i = i >>> 4 | i;
                    i = i >>> 2 | i;
                    i = (i >>> 1 | i) + 1 | 0;
                    i = i >>> 0 > c >>> 0 ? c : i;
                }
                f[k >> 2] = i;
                l = a + 176 | 0;
                i = f[l >> 2] | 0;
                do if (i | 0) {
                    D = f[i + -4 >> 2] | 0;
                    i = i + -8 | 0;
                    if (!((D | 0) != 0 ? (D | 0) == (~f[i >> 2] | 0) : 0)) {
                        f[p >> 2] = 866;
                        f[p + 4 >> 2] = 651;
                        f[p + 8 >> 2] = 1579;
                        vc(E, 812, p) | 0;
                        Ub(E) | 0;
                    }
                    if (!(i & 7)) {
                        Nb(i, 0, 0, 1, 0) | 0;
                        break
                    } else {
                        f[q >> 2] = 866;
                        f[q + 4 >> 2] = 2506;
                        f[q + 8 >> 2] = 1232;
                        vc(E, 812, q) | 0;
                        Ub(E) | 0;
                        break
                    }
                } while (0);
                i = f[k >> 2] | 0;
                i = i | 0 ? i : 1;
                k = Db((i << 1) + 8 | 0, 0) | 0;
                if (!k) {
                    f[l >> 2] = 0;
                    g = 0;
                    break
                } else {
                    f[k + 4 >> 2] = i;
                    f[k >> 2] = ~i;
                    f[l >> 2] = k + 8;
                    s = 24;
                    break
                }
            } else s = 24; while (0);
            a:do if ((s | 0) == 24) {
                D = a + 24 | 0;
                b[D >> 0] = n;
                b[a + 25 >> 0] = m;
                l = a + 176 | 0;
                k = 0;
                do {
                    B = b[e + k >> 0] | 0;
                    i = B & 255;
                    if (B << 24 >> 24) {
                        if (!(f[K + (i << 2) >> 2] | 0)) {
                            f[w >> 2] = 866;
                            f[w + 4 >> 2] = 2276;
                            f[w + 8 >> 2] = 977;
                            vc(E, 812, w) | 0;
                            Ub(E) | 0;
                        }
                        B = t + (i << 2) | 0;
                        i = f[B >> 2] | 0;
                        f[B >> 2] = i + 1;
                        if (i >>> 0 >= r >>> 0) {
                            f[v >> 2] = 866;
                            f[v + 4 >> 2] = 2280;
                            f[v + 8 >> 2] = 990;
                            vc(E, 812, v) | 0;
                            Ub(E) | 0;
                        }
                        d[(f[l >> 2] | 0) + (i << 1) >> 1] = k;
                    }
                    k = k + 1 | 0;
                } while ((k | 0) != (c | 0));
                A = (h[D >> 0] | 0) >>> 0 < g >>> 0 ? g : 0;
                B = a + 8 | 0;
                f[B >> 2] = A;
                z = (A | 0) != 0;
                if (z) {
                    y = 1 << A;
                    i = a + 164 | 0;
                    do if (y >>> 0 > (f[i >> 2] | 0) >>> 0) {
                        f[i >> 2] = y;
                        l = a + 168 | 0;
                        i = f[l >> 2] | 0;
                        do if (i | 0) {
                            w = f[i + -4 >> 2] | 0;
                            i = i + -8 | 0;
                            if (!((w | 0) != 0 ? (w | 0) == (~f[i >> 2] | 0) : 0)) {
                                f[x >> 2] = 866;
                                f[x + 4 >> 2] = 651;
                                f[x + 8 >> 2] = 1579;
                                vc(E, 812, x) | 0;
                                Ub(E) | 0;
                            }
                            if (!(i & 7)) {
                                Nb(i, 0, 0, 1, 0) | 0;
                                break
                            } else {
                                f[C >> 2] = 866;
                                f[C + 4 >> 2] = 2506;
                                f[C + 8 >> 2] = 1232;
                                vc(E, 812, C) | 0;
                                Ub(E) | 0;
                                break
                            }
                        } while (0);
                        i = y << 2;
                        k = Db(i + 8 | 0, 0) | 0;
                        if (!k) {
                            f[l >> 2] = 0;
                            g = 0;
                            break a
                        } else {
                            C = k + 8 | 0;
                            f[k + 4 >> 2] = y;
                            f[k >> 2] = ~y;
                            f[l >> 2] = C;
                            k = C;
                            break
                        }
                    } else {
                        k = a + 168 | 0;
                        i = y << 2;
                        l = k;
                        k = f[k >> 2] | 0;
                    } while (0);
                    Ib(k | 0, -1, i | 0) | 0;
                    v = a + 176 | 0;
                    t = 1;
                    do {
                        if (f[K + (t << 2) >> 2] | 0) {
                            w = A - t | 0;
                            x = 1 << w;
                            i = t + -1 | 0;
                            k = f[F + (i << 2) >> 2] | 0;
                            if (i >>> 0 >= 16) {
                                f[G >> 2] = 866;
                                f[G + 4 >> 2] = 1960;
                                f[G + 8 >> 2] = 1453;
                                vc(E, 812, G) | 0;
                                Ub(E) | 0;
                            }
                            c = f[a + 28 + (i << 2) >> 2] | 0;
                            c = (c | 0) == 0 ? -1 : (c + -1 | 0) >>> (16 - t | 0);
                            if (k >>> 0 <= c >>> 0) {
                                r = (f[a + 96 + (i << 2) >> 2] | 0) - k | 0;
                                s = t << 16;
                                do {
                                    i = j[(f[v >> 2] | 0) + (r + k << 1) >> 1] | 0;
                                    if ((h[e + i >> 0] | 0 | 0) != (t | 0)) {
                                        f[H >> 2] = 866;
                                        f[H + 4 >> 2] = 2322;
                                        f[H + 8 >> 2] = 1019;
                                        vc(E, 812, H) | 0;
                                        Ub(E) | 0;
                                    }
                                    q = k << w;
                                    o = i | s;
                                    n = 0;
                                    do {
                                        p = n + q | 0;
                                        if (p >>> 0 >= y >>> 0) {
                                            f[I >> 2] = 866;
                                            f[I + 4 >> 2] = 2328;
                                            f[I + 8 >> 2] = 1053;
                                            vc(E, 812, I) | 0;
                                            Ub(E) | 0;
                                        }
                                        i = f[l >> 2] | 0;
                                        if ((f[i + (p << 2) >> 2] | 0) != -1) {
                                            f[J >> 2] = 866;
                                            f[J + 4 >> 2] = 2330;
                                            f[J + 8 >> 2] = 1076;
                                            vc(E, 812, J) | 0;
                                            Ub(E) | 0;
                                            i = f[l >> 2] | 0;
                                        }
                                        f[i + (p << 2) >> 2] = o;
                                        n = n + 1 | 0;
                                    } while (n >>> 0 < x >>> 0);
                                    k = k + 1 | 0;
                                } while (k >>> 0 <= c >>> 0)
                            }
                        }
                        t = t + 1 | 0;
                    } while (A >>> 0 >= t >>> 0)
                }
                i = a + 96 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F >> 2] | 0);
                i = a + 100 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 4 >> 2] | 0);
                i = a + 104 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 8 >> 2] | 0);
                i = a + 108 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 12 >> 2] | 0);
                i = a + 112 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 16 >> 2] | 0);
                i = a + 116 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 20 >> 2] | 0);
                i = a + 120 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 24 >> 2] | 0);
                i = a + 124 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 28 >> 2] | 0);
                i = a + 128 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 32 >> 2] | 0);
                i = a + 132 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 36 >> 2] | 0);
                i = a + 136 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 40 >> 2] | 0);
                i = a + 140 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 44 >> 2] | 0);
                i = a + 144 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 48 >> 2] | 0);
                i = a + 148 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 52 >> 2] | 0);
                i = a + 152 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 56 >> 2] | 0);
                i = a + 156 | 0;
                f[i >> 2] = (f[i >> 2] | 0) - (f[F + 60 >> 2] | 0);
                i = a + 16 | 0;
                f[i >> 2] = 0;
                k = a + 20 | 0;
                f[k >> 2] = h[D >> 0];
                b:do if (z) {
                    do {
                        if (!g) break b;
                        J = g;
                        g = g + -1 | 0;
                    } while (!(f[K + (J << 2) >> 2] | 0));
                    f[i >> 2] = f[a + 28 + (g << 2) >> 2];
                    g = A + 1 | 0;
                    f[k >> 2] = g;
                    if (g >>> 0 <= m >>> 0) {
                        while (1) {
                            if (f[K + (g << 2) >> 2] | 0) break;
                            g = g + 1 | 0;
                            if (g >>> 0 > m >>> 0) break b
                        }
                        f[k >> 2] = g;
                    }
                } while (0);
                f[a + 92 >> 2] = -1;
                f[a + 160 >> 2] = 1048575;
                f[a + 12 >> 2] = 32 - (f[B >> 2] | 0);
                g = 1;
            } while (0);
            K = g;
            u = L;
            return K | 0
        }

        function Ra(a, c, d, e, g, i, j, k) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            i = i | 0;
            j = j | 0;
            k = k | 0;
            var l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0, y = 0, z = 0, A = 0,
                B = 0, C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0, J = 0, K = 0, L = 0, M = 0, N = 0, O = 0, P = 0,
                Q = 0, R = 0, S = 0, T = 0, U = 0, V = 0, W = 0, Y = 0, Z = 0, _ = 0, $ = 0, aa = 0, ba = 0, ca = 0,
                da = 0, ea = 0, fa = 0, ga = 0, ha = 0, ia = 0;
            ga = u;
            u = u + 656 | 0;
            ea = ga + 112 | 0;
            ca = ga + 96 | 0;
            ba = ga + 80 | 0;
            aa = ga + 64 | 0;
            $ = ga + 48 | 0;
            fa = ga + 32 | 0;
            da = ga + 16 | 0;
            _ = ga;
            Y = ga + 144 | 0;
            Z = ga + 128 | 0;
            N = a + 240 | 0;
            O = f[N >> 2] | 0;
            P = a + 256 | 0;
            Q = f[P >> 2] | 0;
            W = b[(f[a + 88 >> 2] | 0) + 17 >> 0] | 0;
            R = W & 255;
            S = e >>> 2;
            if (!(W << 24 >> 24)) {
                u = ga;
                return 1
            }
            T = (k | 0) == 0;
            U = j + -1 | 0;
            V = U << 4;
            W = k + -1 | 0;
            H = (i & 1 | 0) != 0;
            I = e << 1;
            J = a + 92 | 0;
            K = a + 116 | 0;
            L = a + 140 | 0;
            M = a + 236 | 0;
            G = (g & 1 | 0) != 0;
            F = a + 188 | 0;
            B = a + 252 | 0;
            C = S + 1 | 0;
            D = S + 2 | 0;
            E = S + 3 | 0;
            A = 0;
            i = 0;
            d = 0;
            g = 1;
            do {
                if (!T) {
                    y = f[c + (A << 2) >> 2] | 0;
                    z = 0;
                    while (1) {
                        w = z & 1;
                        m = (w | 0) == 0;
                        v = (w << 5 ^ 32) + -16 | 0;
                        w = (w << 1 ^ 2) + -1 | 0;
                        t = m ? j : -1;
                        l = m ? 0 : U;
                        a = (z | 0) == (W | 0);
                        x = H & a;
                        if ((l | 0) != (t | 0)) {
                            s = H & a ^ 1;
                            r = m ? y : y + V | 0;
                            while (1) {
                                if ((g | 0) == 1) g = bb(J, K) | 0 | 512;
                                q = g & 7;
                                g = g >>> 3;
                                m = h[1539 + q >> 0] | 0;
                                a = 0;
                                do {
                                    n = (bb(J, L) | 0) + d | 0;
                                    o = n - O | 0;
                                    p = o >> 31;
                                    d = p & n | o & ~p;
                                    if ((f[N >> 2] | 0) >>> 0 <= d >>> 0) {
                                        f[_ >> 2] = 866;
                                        f[_ + 4 >> 2] = 910;
                                        f[_ + 8 >> 2] = 1497;
                                        vc(Y, 812, _) | 0;
                                        Ub(Y) | 0;
                                    }
                                    f[Z + (a << 2) >> 2] = f[(f[M >> 2] | 0) + (d << 2) >> 2];
                                    a = a + 1 | 0;
                                } while (a >>> 0 < m >>> 0);
                                p = G & (l | 0) == (U | 0);
                                if (x | p) {
                                    o = 0;
                                    do {
                                        a = r + (X(o, e) | 0) | 0;
                                        n = (o | 0) == 0 | s;
                                        m = o << 1;
                                        ia = (bb(J, F) | 0) + i | 0;
                                        ha = ia - Q | 0;
                                        i = ha >> 31;
                                        i = i & ia | ha & ~i;
                                        do if (!p) {
                                            if (n) {
                                                f[a >> 2] = f[Z + ((h[1547 + (q << 2) + m >> 0] | 0) << 2) >> 2];
                                                if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                                    f[ba >> 2] = 866;
                                                    f[ba + 4 >> 2] = 910;
                                                    f[ba + 8 >> 2] = 1497;
                                                    vc(Y, 812, ba) | 0;
                                                    Ub(Y) | 0;
                                                }
                                                f[a + 4 >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                            }
                                            a = a + 8 | 0;
                                            ha = (bb(J, F) | 0) + i | 0;
                                            ia = ha - Q | 0;
                                            i = ia >> 31;
                                            i = i & ha | ia & ~i;
                                            if (n) {
                                                f[a >> 2] = f[Z + ((h[(m | 1) + (1547 + (q << 2)) >> 0] | 0) << 2) >> 2];
                                                if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                                    f[ea >> 2] = 866;
                                                    f[ea + 4 >> 2] = 910;
                                                    f[ea + 8 >> 2] = 1497;
                                                    vc(Y, 812, ea) | 0;
                                                    Ub(Y) | 0;
                                                }
                                                f[a + 4 >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                            }
                                        } else {
                                            if (!n) {
                                                ha = (bb(J, F) | 0) + i | 0;
                                                ia = ha - Q | 0;
                                                i = ia >> 31;
                                                i = i & ha | ia & ~i;
                                                break
                                            }
                                            f[a >> 2] = f[Z + ((h[1547 + (q << 2) + m >> 0] | 0) << 2) >> 2];
                                            if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                                f[ca >> 2] = 866;
                                                f[ca + 4 >> 2] = 910;
                                                f[ca + 8 >> 2] = 1497;
                                                vc(Y, 812, ca) | 0;
                                                Ub(Y) | 0;
                                            }
                                            f[a + 4 >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                            ha = (bb(J, F) | 0) + i | 0;
                                            ia = ha - Q | 0;
                                            i = ia >> 31;
                                            i = i & ha | ia & ~i;
                                        } while (0);
                                        o = o + 1 | 0;
                                    } while ((o | 0) != 2)
                                } else {
                                    f[r >> 2] = f[Z + ((h[1547 + (q << 2) >> 0] | 0) << 2) >> 2];
                                    ha = (bb(J, F) | 0) + i | 0;
                                    ia = ha - Q | 0;
                                    i = ia >> 31;
                                    i = i & ha | ia & ~i;
                                    if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                        f[da >> 2] = 866;
                                        f[da + 4 >> 2] = 910;
                                        f[da + 8 >> 2] = 1497;
                                        vc(Y, 812, da) | 0;
                                        Ub(Y) | 0;
                                    }
                                    f[r + 4 >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                    f[r + 8 >> 2] = f[Z + ((h[1547 + (q << 2) + 1 >> 0] | 0) << 2) >> 2];
                                    ha = (bb(J, F) | 0) + i | 0;
                                    ia = ha - Q | 0;
                                    i = ia >> 31;
                                    i = i & ha | ia & ~i;
                                    if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                        f[fa >> 2] = 866;
                                        f[fa + 4 >> 2] = 910;
                                        f[fa + 8 >> 2] = 1497;
                                        vc(Y, 812, fa) | 0;
                                        Ub(Y) | 0;
                                    }
                                    f[r + 12 >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                    f[r + (S << 2) >> 2] = f[Z + ((h[1547 + (q << 2) + 2 >> 0] | 0) << 2) >> 2];
                                    ha = (bb(J, F) | 0) + i | 0;
                                    ia = ha - Q | 0;
                                    i = ia >> 31;
                                    i = i & ha | ia & ~i;
                                    if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                        f[$ >> 2] = 866;
                                        f[$ + 4 >> 2] = 910;
                                        f[$ + 8 >> 2] = 1497;
                                        vc(Y, 812, $) | 0;
                                        Ub(Y) | 0;
                                    }
                                    f[r + (C << 2) >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                    f[r + (D << 2) >> 2] = f[Z + ((h[1547 + (q << 2) + 3 >> 0] | 0) << 2) >> 2];
                                    ha = (bb(J, F) | 0) + i | 0;
                                    ia = ha - Q | 0;
                                    i = ia >> 31;
                                    i = i & ha | ia & ~i;
                                    if ((f[P >> 2] | 0) >>> 0 <= i >>> 0) {
                                        f[aa >> 2] = 866;
                                        f[aa + 4 >> 2] = 910;
                                        f[aa + 8 >> 2] = 1497;
                                        vc(Y, 812, aa) | 0;
                                        Ub(Y) | 0;
                                    }
                                    f[r + (E << 2) >> 2] = f[(f[B >> 2] | 0) + (i << 2) >> 2];
                                }
                                l = w + l | 0;
                                if ((l | 0) == (t | 0)) break; else r = r + v | 0;
                            }
                        }
                        z = z + 1 | 0;
                        if ((z | 0) == (k | 0)) break; else y = y + I | 0;
                    }
                }
                A = A + 1 | 0;
            } while ((A | 0) != (R | 0));
            u = ga;
            return 1
        }

        function Sa(a, c, d, e, g, i, k, l) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            i = i | 0;
            k = k | 0;
            l = l | 0;
            var m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0, y = 0, z = 0, A = 0, B = 0,
                C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0, J = 0, K = 0, L = 0, M = 0, N = 0, O = 0, P = 0, Q = 0,
                R = 0, S = 0, T = 0, U = 0, V = 0, W = 0, X = 0, Y = 0, Z = 0, _ = 0, $ = 0, aa = 0, ba = 0, ca = 0,
                da = 0, ea = 0, fa = 0, ga = 0, ha = 0, ia = 0, ja = 0, ka = 0;
            ka = u;
            u = u + 640 | 0;
            ha = ka + 80 | 0;
            ga = ka + 64 | 0;
            fa = ka + 48 | 0;
            ja = ka + 32 | 0;
            ia = ka + 16 | 0;
            ea = ka;
            ca = ka + 128 | 0;
            da = ka + 112 | 0;
            P = ka + 96 | 0;
            Q = a + 272 | 0;
            R = f[Q >> 2] | 0;
            ba = f[a + 88 >> 2] | 0;
            S = (h[ba + 63 >> 0] | 0) << 8 | (h[ba + 64 >> 0] | 0);
            ba = b[ba + 17 >> 0] | 0;
            T = ba & 255;
            if (!(ba << 24 >> 24)) {
                u = ka;
                return 1
            }
            U = (l | 0) == 0;
            V = k + -1 | 0;
            W = V << 5;
            X = l + -1 | 0;
            Y = e << 1;
            Z = a + 92 | 0;
            _ = a + 116 | 0;
            $ = a + 164 | 0;
            aa = a + 268 | 0;
            ba = a + 212 | 0;
            O = (g & 1 | 0) == 0;
            N = (i & 1 | 0) == 0;
            M = a + 288 | 0;
            L = a + 284 | 0;
            K = 0;
            a = 0;
            i = 0;
            g = 0;
            d = 0;
            m = 1;
            do {
                if (!U) {
                    I = f[c + (K << 2) >> 2] | 0;
                    J = 0;
                    while (1) {
                        H = J & 1;
                        o = (H | 0) == 0;
                        G = (H << 6 ^ 64) + -32 | 0;
                        H = (H << 1 ^ 2) + -1 | 0;
                        E = o ? k : -1;
                        n = o ? 0 : V;
                        if ((n | 0) != (E | 0)) {
                            F = N | (J | 0) != (X | 0);
                            D = o ? I : I + W | 0;
                            while (1) {
                                if ((m | 0) == 1) m = bb(Z, _) | 0 | 512;
                                C = m & 7;
                                m = m >>> 3;
                                p = h[1539 + C >> 0] | 0;
                                o = 0;
                                do {
                                    z = (bb(Z, $) | 0) + d | 0;
                                    A = z - R | 0;
                                    B = A >> 31;
                                    d = B & z | A & ~B;
                                    if ((f[Q >> 2] | 0) >>> 0 <= d >>> 0) {
                                        f[ea >> 2] = 866;
                                        f[ea + 4 >> 2] = 910;
                                        f[ea + 8 >> 2] = 1497;
                                        vc(ca, 812, ea) | 0;
                                        Ub(ca) | 0;
                                    }
                                    f[da + (o << 2) >> 2] = j[(f[aa >> 2] | 0) + (d << 1) >> 1];
                                    o = o + 1 | 0;
                                } while (o >>> 0 < p >>> 0);
                                o = 0;
                                do {
                                    z = (bb(Z, $) | 0) + i | 0;
                                    A = z - R | 0;
                                    B = A >> 31;
                                    i = B & z | A & ~B;
                                    if ((f[Q >> 2] | 0) >>> 0 <= i >>> 0) {
                                        f[ia >> 2] = 866;
                                        f[ia + 4 >> 2] = 910;
                                        f[ia + 8 >> 2] = 1497;
                                        vc(ca, 812, ia) | 0;
                                        Ub(ca) | 0;
                                    }
                                    f[P + (o << 2) >> 2] = j[(f[aa >> 2] | 0) + (i << 1) >> 1];
                                    o = o + 1 | 0;
                                } while (o >>> 0 < p >>> 0);
                                B = O | (n | 0) != (V | 0);
                                z = 0;
                                A = D;
                                while (1) {
                                    w = F | (z | 0) == 0;
                                    x = z << 1;
                                    if (B) {
                                        t = 0;
                                        v = A;
                                        while (1) {
                                            y = (bb(Z, ba) | 0) + g | 0;
                                            s = y - S | 0;
                                            g = s >> 31;
                                            g = g & y | s & ~g;
                                            s = (bb(Z, ba) | 0) + a | 0;
                                            y = s - S | 0;
                                            a = y >> 31;
                                            a = a & s | y & ~a;
                                            if (w) {
                                                s = h[t + x + (1547 + (C << 2)) >> 0] | 0;
                                                p = g * 3 | 0;
                                                o = f[M >> 2] | 0;
                                                if (o >>> 0 <= p >>> 0) {
                                                    f[ja >> 2] = 866;
                                                    f[ja + 4 >> 2] = 910;
                                                    f[ja + 8 >> 2] = 1497;
                                                    vc(ca, 812, ja) | 0;
                                                    Ub(ca) | 0;
                                                    o = f[M >> 2] | 0;
                                                }
                                                q = f[L >> 2] | 0;
                                                p = q + (p << 1) | 0;
                                                r = a * 3 | 0;
                                                if (o >>> 0 > r >>> 0) o = q; else {
                                                    f[fa >> 2] = 866;
                                                    f[fa + 4 >> 2] = 910;
                                                    f[fa + 8 >> 2] = 1497;
                                                    vc(ca, 812, fa) | 0;
                                                    Ub(ca) | 0;
                                                    o = f[L >> 2] | 0;
                                                }
                                                y = o + (r << 1) | 0;
                                                f[v >> 2] = (j[p >> 1] | 0) << 16 | f[da + (s << 2) >> 2];
                                                f[v + 4 >> 2] = (j[p + 4 >> 1] | 0) << 16 | (j[p + 2 >> 1] | 0);
                                                f[v + 8 >> 2] = (j[y >> 1] | 0) << 16 | f[P + (s << 2) >> 2];
                                                f[v + 12 >> 2] = (j[y + 4 >> 1] | 0) << 16 | (j[y + 2 >> 1] | 0);
                                            }
                                            t = t + 1 | 0;
                                            if ((t | 0) == 2) break; else v = v + 16 | 0;
                                        }
                                    } else {
                                        y = w ^ 1;
                                        w = 1547 + (C << 2) + x | 0;
                                        t = 0;
                                        v = A;
                                        while (1) {
                                            x = (bb(Z, ba) | 0) + g | 0;
                                            s = x - S | 0;
                                            g = s >> 31;
                                            g = g & x | s & ~g;
                                            s = (bb(Z, ba) | 0) + a | 0;
                                            x = s - S | 0;
                                            a = x >> 31;
                                            a = a & s | x & ~a;
                                            if (!((t | 0) != 0 | y)) {
                                                s = h[w >> 0] | 0;
                                                p = g * 3 | 0;
                                                o = f[M >> 2] | 0;
                                                if (o >>> 0 <= p >>> 0) {
                                                    f[ga >> 2] = 866;
                                                    f[ga + 4 >> 2] = 910;
                                                    f[ga + 8 >> 2] = 1497;
                                                    vc(ca, 812, ga) | 0;
                                                    Ub(ca) | 0;
                                                    o = f[M >> 2] | 0;
                                                }
                                                q = f[L >> 2] | 0;
                                                p = q + (p << 1) | 0;
                                                r = a * 3 | 0;
                                                if (o >>> 0 > r >>> 0) o = q; else {
                                                    f[ha >> 2] = 866;
                                                    f[ha + 4 >> 2] = 910;
                                                    f[ha + 8 >> 2] = 1497;
                                                    vc(ca, 812, ha) | 0;
                                                    Ub(ca) | 0;
                                                    o = f[L >> 2] | 0;
                                                }
                                                x = o + (r << 1) | 0;
                                                f[v >> 2] = (j[p >> 1] | 0) << 16 | f[da + (s << 2) >> 2];
                                                f[v + 4 >> 2] = (j[p + 4 >> 1] | 0) << 16 | (j[p + 2 >> 1] | 0);
                                                f[v + 8 >> 2] = (j[x >> 1] | 0) << 16 | f[P + (s << 2) >> 2];
                                                f[v + 12 >> 2] = (j[x + 4 >> 1] | 0) << 16 | (j[x + 2 >> 1] | 0);
                                            }
                                            t = t + 1 | 0;
                                            if ((t | 0) == 2) break; else v = v + 16 | 0;
                                        }
                                    }
                                    z = z + 1 | 0;
                                    if ((z | 0) == 2) break; else A = A + e | 0;
                                }
                                n = H + n | 0;
                                if ((n | 0) == (E | 0)) break; else D = D + G | 0;
                            }
                        }
                        J = J + 1 | 0;
                        if ((J | 0) == (l | 0)) break; else I = I + Y | 0;
                    }
                }
                K = K + 1 | 0;
            } while ((K | 0) != (T | 0));
            u = ka;
            return 1
        }

        function Ta(a) {
            a = a | 0;
            var b = 0, c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0;
            if (!a) return;
            c = a + -8 | 0;
            e = f[1148] | 0;
            a = f[a + -4 >> 2] | 0;
            b = a & -8;
            j = c + b | 0;
            do if (!(a & 1)) {
                d = f[c >> 2] | 0;
                if (!(a & 3)) return;
                h = c + (0 - d) | 0;
                g = d + b | 0;
                if (h >>> 0 < e >>> 0) return;
                if ((h | 0) == (f[1149] | 0)) {
                    a = j + 4 | 0;
                    b = f[a >> 2] | 0;
                    if ((b & 3 | 0) != 3) {
                        i = h;
                        b = g;
                        break
                    }
                    f[1146] = g;
                    f[a >> 2] = b & -2;
                    f[h + 4 >> 2] = g | 1;
                    f[h + g >> 2] = g;
                    return
                }
                c = d >>> 3;
                if (d >>> 0 < 256) {
                    a = f[h + 8 >> 2] | 0;
                    b = f[h + 12 >> 2] | 0;
                    if ((b | 0) == (a | 0)) {
                        f[1144] = f[1144] & ~(1 << c);
                        i = h;
                        b = g;
                        break
                    } else {
                        f[a + 12 >> 2] = b;
                        f[b + 8 >> 2] = a;
                        i = h;
                        b = g;
                        break
                    }
                }
                e = f[h + 24 >> 2] | 0;
                a = f[h + 12 >> 2] | 0;
                do if ((a | 0) == (h | 0)) {
                    c = h + 16 | 0;
                    b = c + 4 | 0;
                    a = f[b >> 2] | 0;
                    if (!a) {
                        a = f[c >> 2] | 0;
                        if (!a) {
                            a = 0;
                            break
                        } else b = c;
                    }
                    while (1) {
                        c = a + 20 | 0;
                        d = f[c >> 2] | 0;
                        if (d | 0) {
                            a = d;
                            b = c;
                            continue
                        }
                        c = a + 16 | 0;
                        d = f[c >> 2] | 0;
                        if (!d) break; else {
                            a = d;
                            b = c;
                        }
                    }
                    f[b >> 2] = 0;
                } else {
                    i = f[h + 8 >> 2] | 0;
                    f[i + 12 >> 2] = a;
                    f[a + 8 >> 2] = i;
                } while (0);
                if (e) {
                    b = f[h + 28 >> 2] | 0;
                    c = 4880 + (b << 2) | 0;
                    if ((h | 0) == (f[c >> 2] | 0)) {
                        f[c >> 2] = a;
                        if (!a) {
                            f[1145] = f[1145] & ~(1 << b);
                            i = h;
                            b = g;
                            break
                        }
                    } else {
                        f[e + 16 + (((f[e + 16 >> 2] | 0) != (h | 0) & 1) << 2) >> 2] = a;
                        if (!a) {
                            i = h;
                            b = g;
                            break
                        }
                    }
                    f[a + 24 >> 2] = e;
                    b = h + 16 | 0;
                    c = f[b >> 2] | 0;
                    if (c | 0) {
                        f[a + 16 >> 2] = c;
                        f[c + 24 >> 2] = a;
                    }
                    b = f[b + 4 >> 2] | 0;
                    if (b) {
                        f[a + 20 >> 2] = b;
                        f[b + 24 >> 2] = a;
                        i = h;
                        b = g;
                    } else {
                        i = h;
                        b = g;
                    }
                } else {
                    i = h;
                    b = g;
                }
            } else {
                i = c;
                h = c;
            } while (0);
            if (h >>> 0 >= j >>> 0) return;
            a = j + 4 | 0;
            d = f[a >> 2] | 0;
            if (!(d & 1)) return;
            if (!(d & 2)) {
                a = f[1149] | 0;
                if ((j | 0) == (f[1150] | 0)) {
                    j = (f[1147] | 0) + b | 0;
                    f[1147] = j;
                    f[1150] = i;
                    f[i + 4 >> 2] = j | 1;
                    if ((i | 0) != (a | 0)) return;
                    f[1149] = 0;
                    f[1146] = 0;
                    return
                }
                if ((j | 0) == (a | 0)) {
                    j = (f[1146] | 0) + b | 0;
                    f[1146] = j;
                    f[1149] = h;
                    f[i + 4 >> 2] = j | 1;
                    f[h + j >> 2] = j;
                    return
                }
                e = (d & -8) + b | 0;
                c = d >>> 3;
                do if (d >>> 0 < 256) {
                    b = f[j + 8 >> 2] | 0;
                    a = f[j + 12 >> 2] | 0;
                    if ((a | 0) == (b | 0)) {
                        f[1144] = f[1144] & ~(1 << c);
                        break
                    } else {
                        f[b + 12 >> 2] = a;
                        f[a + 8 >> 2] = b;
                        break
                    }
                } else {
                    g = f[j + 24 >> 2] | 0;
                    a = f[j + 12 >> 2] | 0;
                    do if ((a | 0) == (j | 0)) {
                        c = j + 16 | 0;
                        b = c + 4 | 0;
                        a = f[b >> 2] | 0;
                        if (!a) {
                            a = f[c >> 2] | 0;
                            if (!a) {
                                c = 0;
                                break
                            } else b = c;
                        }
                        while (1) {
                            c = a + 20 | 0;
                            d = f[c >> 2] | 0;
                            if (d | 0) {
                                a = d;
                                b = c;
                                continue
                            }
                            c = a + 16 | 0;
                            d = f[c >> 2] | 0;
                            if (!d) break; else {
                                a = d;
                                b = c;
                            }
                        }
                        f[b >> 2] = 0;
                        c = a;
                    } else {
                        c = f[j + 8 >> 2] | 0;
                        f[c + 12 >> 2] = a;
                        f[a + 8 >> 2] = c;
                        c = a;
                    } while (0);
                    if (g | 0) {
                        a = f[j + 28 >> 2] | 0;
                        b = 4880 + (a << 2) | 0;
                        if ((j | 0) == (f[b >> 2] | 0)) {
                            f[b >> 2] = c;
                            if (!c) {
                                f[1145] = f[1145] & ~(1 << a);
                                break
                            }
                        } else {
                            f[g + 16 + (((f[g + 16 >> 2] | 0) != (j | 0) & 1) << 2) >> 2] = c;
                            if (!c) break
                        }
                        f[c + 24 >> 2] = g;
                        a = j + 16 | 0;
                        b = f[a >> 2] | 0;
                        if (b | 0) {
                            f[c + 16 >> 2] = b;
                            f[b + 24 >> 2] = c;
                        }
                        a = f[a + 4 >> 2] | 0;
                        if (a | 0) {
                            f[c + 20 >> 2] = a;
                            f[a + 24 >> 2] = c;
                        }
                    }
                } while (0);
                f[i + 4 >> 2] = e | 1;
                f[h + e >> 2] = e;
                if ((i | 0) == (f[1149] | 0)) {
                    f[1146] = e;
                    return
                }
            } else {
                f[a >> 2] = d & -2;
                f[i + 4 >> 2] = b | 1;
                f[h + b >> 2] = b;
                e = b;
            }
            a = e >>> 3;
            if (e >>> 0 < 256) {
                c = 4616 + (a << 1 << 2) | 0;
                b = f[1144] | 0;
                a = 1 << a;
                if (!(b & a)) {
                    f[1144] = b | a;
                    a = c;
                    b = c + 8 | 0;
                } else {
                    b = c + 8 | 0;
                    a = f[b >> 2] | 0;
                }
                f[b >> 2] = i;
                f[a + 12 >> 2] = i;
                f[i + 8 >> 2] = a;
                f[i + 12 >> 2] = c;
                return
            }
            a = e >>> 8;
            if (a) if (e >>> 0 > 16777215) a = 31; else {
                h = (a + 1048320 | 0) >>> 16 & 8;
                j = a << h;
                g = (j + 520192 | 0) >>> 16 & 4;
                j = j << g;
                a = (j + 245760 | 0) >>> 16 & 2;
                a = 14 - (g | h | a) + (j << a >>> 15) | 0;
                a = e >>> (a + 7 | 0) & 1 | a << 1;
            } else a = 0;
            d = 4880 + (a << 2) | 0;
            f[i + 28 >> 2] = a;
            f[i + 20 >> 2] = 0;
            f[i + 16 >> 2] = 0;
            b = f[1145] | 0;
            c = 1 << a;
            do if (b & c) {
                b = e << ((a | 0) == 31 ? 0 : 25 - (a >>> 1) | 0);
                c = f[d >> 2] | 0;
                while (1) {
                    if ((f[c + 4 >> 2] & -8 | 0) == (e | 0)) {
                        a = 73;
                        break
                    }
                    d = c + 16 + (b >>> 31 << 2) | 0;
                    a = f[d >> 2] | 0;
                    if (!a) {
                        a = 72;
                        break
                    } else {
                        b = b << 1;
                        c = a;
                    }
                }
                if ((a | 0) == 72) {
                    f[d >> 2] = i;
                    f[i + 24 >> 2] = c;
                    f[i + 12 >> 2] = i;
                    f[i + 8 >> 2] = i;
                    break
                } else if ((a | 0) == 73) {
                    h = c + 8 | 0;
                    j = f[h >> 2] | 0;
                    f[j + 12 >> 2] = i;
                    f[h >> 2] = i;
                    f[i + 8 >> 2] = j;
                    f[i + 12 >> 2] = c;
                    f[i + 24 >> 2] = 0;
                    break
                }
            } else {
                f[1145] = b | c;
                f[d >> 2] = i;
                f[i + 24 >> 2] = d;
                f[i + 12 >> 2] = i;
                f[i + 8 >> 2] = i;
            } while (0);
            j = (f[1152] | 0) + -1 | 0;
            f[1152] = j;
            if (!j) a = 5032; else return;
            while (1) {
                a = f[a >> 2] | 0;
                if (!a) break; else a = a + 8 | 0;
            }
            f[1152] = -1;
        }

        function Ua(a, c, d, e, g, i, k, l) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            i = i | 0;
            k = k | 0;
            l = l | 0;
            var m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0, y = 0, z = 0, A = 0, B = 0,
                C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0, J = 0, K = 0, L = 0, M = 0, N = 0, O = 0, P = 0, Q = 0,
                R = 0, S = 0, T = 0, U = 0, V = 0, W = 0, X = 0, Y = 0, Z = 0, _ = 0, $ = 0, aa = 0, ba = 0, ca = 0,
                da = 0, ea = 0, fa = 0, ga = 0, ha = 0, ia = 0, ja = 0, ka = 0, la = 0, ma = 0, na = 0, oa = 0, pa = 0;
            pa = u;
            u = u + 640 | 0;
            ma = pa + 80 | 0;
            la = pa + 64 | 0;
            ka = pa + 48 | 0;
            oa = pa + 32 | 0;
            na = pa + 16 | 0;
            ja = pa;
            ha = pa + 128 | 0;
            ia = pa + 112 | 0;
            N = pa + 96 | 0;
            O = a + 240 | 0;
            P = f[O >> 2] | 0;
            Q = a + 256 | 0;
            R = f[Q >> 2] | 0;
            S = a + 272 | 0;
            T = f[S >> 2] | 0;
            ga = f[a + 88 >> 2] | 0;
            U = (h[ga + 63 >> 0] | 0) << 8 | (h[ga + 64 >> 0] | 0);
            ga = b[ga + 17 >> 0] | 0;
            V = ga & 255;
            if (!(ga << 24 >> 24)) {
                u = pa;
                return 1
            }
            W = (l | 0) == 0;
            X = k + -1 | 0;
            Y = X << 5;
            Z = l + -1 | 0;
            _ = e << 1;
            $ = a + 92 | 0;
            aa = a + 116 | 0;
            ba = a + 164 | 0;
            ca = a + 268 | 0;
            da = a + 140 | 0;
            ea = a + 236 | 0;
            fa = a + 212 | 0;
            ga = a + 188 | 0;
            M = (g & 1 | 0) == 0;
            L = (i & 1 | 0) == 0;
            J = a + 288 | 0;
            K = a + 284 | 0;
            I = a + 252 | 0;
            H = 0;
            a = 0;
            i = 0;
            g = 0;
            d = 0;
            m = 1;
            do {
                if (!W) {
                    F = f[c + (H << 2) >> 2] | 0;
                    G = 0;
                    while (1) {
                        E = G & 1;
                        o = (E | 0) == 0;
                        D = (E << 6 ^ 64) + -32 | 0;
                        E = (E << 1 ^ 2) + -1 | 0;
                        B = o ? k : -1;
                        n = o ? 0 : X;
                        if ((n | 0) != (B | 0)) {
                            C = L | (G | 0) != (Z | 0);
                            A = o ? F : F + Y | 0;
                            while (1) {
                                if ((m | 0) == 1) m = bb($, aa) | 0 | 512;
                                z = m & 7;
                                m = m >>> 3;
                                p = h[1539 + z >> 0] | 0;
                                o = 0;
                                do {
                                    w = (bb($, ba) | 0) + i | 0;
                                    x = w - T | 0;
                                    y = x >> 31;
                                    i = y & w | x & ~y;
                                    if ((f[S >> 2] | 0) >>> 0 <= i >>> 0) {
                                        f[ja >> 2] = 866;
                                        f[ja + 4 >> 2] = 910;
                                        f[ja + 8 >> 2] = 1497;
                                        vc(ha, 812, ja) | 0;
                                        Ub(ha) | 0;
                                    }
                                    f[N + (o << 2) >> 2] = j[(f[ca >> 2] | 0) + (i << 1) >> 1];
                                    o = o + 1 | 0;
                                } while (o >>> 0 < p >>> 0);
                                o = 0;
                                do {
                                    w = (bb($, da) | 0) + d | 0;
                                    x = w - P | 0;
                                    y = x >> 31;
                                    d = y & w | x & ~y;
                                    if ((f[O >> 2] | 0) >>> 0 <= d >>> 0) {
                                        f[na >> 2] = 866;
                                        f[na + 4 >> 2] = 910;
                                        f[na + 8 >> 2] = 1497;
                                        vc(ha, 812, na) | 0;
                                        Ub(ha) | 0;
                                    }
                                    f[ia + (o << 2) >> 2] = f[(f[ea >> 2] | 0) + (d << 2) >> 2];
                                    o = o + 1 | 0;
                                } while (o >>> 0 < p >>> 0);
                                y = M | (n | 0) != (X | 0);
                                w = 0;
                                x = A;
                                while (1) {
                                    s = C | (w | 0) == 0;
                                    t = w << 1;
                                    if (y) {
                                        q = 0;
                                        r = x;
                                        while (1) {
                                            v = (bb($, fa) | 0) + a | 0;
                                            p = v - U | 0;
                                            a = p >> 31;
                                            a = a & v | p & ~a;
                                            p = (bb($, ga) | 0) + g | 0;
                                            v = p - R | 0;
                                            g = v >> 31;
                                            g = g & p | v & ~g;
                                            if (s) {
                                                o = h[q + t + (1547 + (z << 2)) >> 0] | 0;
                                                p = a * 3 | 0;
                                                if ((f[J >> 2] | 0) >>> 0 <= p >>> 0) {
                                                    f[oa >> 2] = 866;
                                                    f[oa + 4 >> 2] = 910;
                                                    f[oa + 8 >> 2] = 1497;
                                                    vc(ha, 812, oa) | 0;
                                                    Ub(ha) | 0;
                                                }
                                                v = (f[K >> 2] | 0) + (p << 1) | 0;
                                                f[r >> 2] = (j[v >> 1] | 0) << 16 | f[N + (o << 2) >> 2];
                                                f[r + 4 >> 2] = (j[v + 4 >> 1] | 0) << 16 | (j[v + 2 >> 1] | 0);
                                                f[r + 8 >> 2] = f[ia + (o << 2) >> 2];
                                                if ((f[Q >> 2] | 0) >>> 0 <= g >>> 0) {
                                                    f[ka >> 2] = 866;
                                                    f[ka + 4 >> 2] = 910;
                                                    f[ka + 8 >> 2] = 1497;
                                                    vc(ha, 812, ka) | 0;
                                                    Ub(ha) | 0;
                                                }
                                                f[r + 12 >> 2] = f[(f[I >> 2] | 0) + (g << 2) >> 2];
                                            }
                                            q = q + 1 | 0;
                                            if ((q | 0) == 2) break; else r = r + 16 | 0;
                                        }
                                    } else {
                                        v = s ^ 1;
                                        s = 1547 + (z << 2) + t | 0;
                                        q = 0;
                                        r = x;
                                        while (1) {
                                            t = (bb($, fa) | 0) + a | 0;
                                            p = t - U | 0;
                                            a = p >> 31;
                                            a = a & t | p & ~a;
                                            p = (bb($, ga) | 0) + g | 0;
                                            t = p - R | 0;
                                            g = t >> 31;
                                            g = g & p | t & ~g;
                                            if (!((q | 0) != 0 | v)) {
                                                o = h[s >> 0] | 0;
                                                p = a * 3 | 0;
                                                if ((f[J >> 2] | 0) >>> 0 <= p >>> 0) {
                                                    f[la >> 2] = 866;
                                                    f[la + 4 >> 2] = 910;
                                                    f[la + 8 >> 2] = 1497;
                                                    vc(ha, 812, la) | 0;
                                                    Ub(ha) | 0;
                                                }
                                                t = (f[K >> 2] | 0) + (p << 1) | 0;
                                                f[r >> 2] = (j[t >> 1] | 0) << 16 | f[N + (o << 2) >> 2];
                                                f[r + 4 >> 2] = (j[t + 4 >> 1] | 0) << 16 | (j[t + 2 >> 1] | 0);
                                                f[r + 8 >> 2] = f[ia + (o << 2) >> 2];
                                                if ((f[Q >> 2] | 0) >>> 0 <= g >>> 0) {
                                                    f[ma >> 2] = 866;
                                                    f[ma + 4 >> 2] = 910;
                                                    f[ma + 8 >> 2] = 1497;
                                                    vc(ha, 812, ma) | 0;
                                                    Ub(ha) | 0;
                                                }
                                                f[r + 12 >> 2] = f[(f[I >> 2] | 0) + (g << 2) >> 2];
                                            }
                                            q = q + 1 | 0;
                                            if ((q | 0) == 2) break; else r = r + 16 | 0;
                                        }
                                    }
                                    w = w + 1 | 0;
                                    if ((w | 0) == 2) break; else x = x + e | 0;
                                }
                                n = E + n | 0;
                                if ((n | 0) == (B | 0)) break; else A = A + D | 0;
                            }
                        }
                        G = G + 1 | 0;
                        if ((G | 0) == (l | 0)) break; else F = F + _ | 0;
                    }
                }
                H = H + 1 | 0;
            } while ((H | 0) != (V | 0));
            u = pa;
            return 1
        }

        function Va(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0;
            j = a + b | 0;
            c = f[a + 4 >> 2] | 0;
            do if (!(c & 1)) {
                d = f[a >> 2] | 0;
                if (!(c & 3)) return;
                g = a + (0 - d) | 0;
                h = d + b | 0;
                if ((g | 0) == (f[1149] | 0)) {
                    a = j + 4 | 0;
                    c = f[a >> 2] | 0;
                    if ((c & 3 | 0) != 3) {
                        i = g;
                        c = h;
                        break
                    }
                    f[1146] = h;
                    f[a >> 2] = c & -2;
                    f[g + 4 >> 2] = h | 1;
                    f[g + h >> 2] = h;
                    return
                }
                b = d >>> 3;
                if (d >>> 0 < 256) {
                    a = f[g + 8 >> 2] | 0;
                    c = f[g + 12 >> 2] | 0;
                    if ((c | 0) == (a | 0)) {
                        f[1144] = f[1144] & ~(1 << b);
                        i = g;
                        c = h;
                        break
                    } else {
                        f[a + 12 >> 2] = c;
                        f[c + 8 >> 2] = a;
                        i = g;
                        c = h;
                        break
                    }
                }
                e = f[g + 24 >> 2] | 0;
                a = f[g + 12 >> 2] | 0;
                do if ((a | 0) == (g | 0)) {
                    b = g + 16 | 0;
                    c = b + 4 | 0;
                    a = f[c >> 2] | 0;
                    if (!a) {
                        a = f[b >> 2] | 0;
                        if (!a) {
                            a = 0;
                            break
                        } else c = b;
                    }
                    while (1) {
                        b = a + 20 | 0;
                        d = f[b >> 2] | 0;
                        if (d | 0) {
                            a = d;
                            c = b;
                            continue
                        }
                        b = a + 16 | 0;
                        d = f[b >> 2] | 0;
                        if (!d) break; else {
                            a = d;
                            c = b;
                        }
                    }
                    f[c >> 2] = 0;
                } else {
                    i = f[g + 8 >> 2] | 0;
                    f[i + 12 >> 2] = a;
                    f[a + 8 >> 2] = i;
                } while (0);
                if (e) {
                    c = f[g + 28 >> 2] | 0;
                    b = 4880 + (c << 2) | 0;
                    if ((g | 0) == (f[b >> 2] | 0)) {
                        f[b >> 2] = a;
                        if (!a) {
                            f[1145] = f[1145] & ~(1 << c);
                            i = g;
                            c = h;
                            break
                        }
                    } else {
                        f[e + 16 + (((f[e + 16 >> 2] | 0) != (g | 0) & 1) << 2) >> 2] = a;
                        if (!a) {
                            i = g;
                            c = h;
                            break
                        }
                    }
                    f[a + 24 >> 2] = e;
                    c = g + 16 | 0;
                    b = f[c >> 2] | 0;
                    if (b | 0) {
                        f[a + 16 >> 2] = b;
                        f[b + 24 >> 2] = a;
                    }
                    c = f[c + 4 >> 2] | 0;
                    if (c) {
                        f[a + 20 >> 2] = c;
                        f[c + 24 >> 2] = a;
                        i = g;
                        c = h;
                    } else {
                        i = g;
                        c = h;
                    }
                } else {
                    i = g;
                    c = h;
                }
            } else {
                i = a;
                c = b;
            } while (0);
            a = j + 4 | 0;
            d = f[a >> 2] | 0;
            if (!(d & 2)) {
                a = f[1149] | 0;
                if ((j | 0) == (f[1150] | 0)) {
                    j = (f[1147] | 0) + c | 0;
                    f[1147] = j;
                    f[1150] = i;
                    f[i + 4 >> 2] = j | 1;
                    if ((i | 0) != (a | 0)) return;
                    f[1149] = 0;
                    f[1146] = 0;
                    return
                }
                if ((j | 0) == (a | 0)) {
                    j = (f[1146] | 0) + c | 0;
                    f[1146] = j;
                    f[1149] = i;
                    f[i + 4 >> 2] = j | 1;
                    f[i + j >> 2] = j;
                    return
                }
                g = (d & -8) + c | 0;
                b = d >>> 3;
                do if (d >>> 0 < 256) {
                    c = f[j + 8 >> 2] | 0;
                    a = f[j + 12 >> 2] | 0;
                    if ((a | 0) == (c | 0)) {
                        f[1144] = f[1144] & ~(1 << b);
                        break
                    } else {
                        f[c + 12 >> 2] = a;
                        f[a + 8 >> 2] = c;
                        break
                    }
                } else {
                    e = f[j + 24 >> 2] | 0;
                    a = f[j + 12 >> 2] | 0;
                    do if ((a | 0) == (j | 0)) {
                        b = j + 16 | 0;
                        c = b + 4 | 0;
                        a = f[c >> 2] | 0;
                        if (!a) {
                            a = f[b >> 2] | 0;
                            if (!a) {
                                b = 0;
                                break
                            } else c = b;
                        }
                        while (1) {
                            b = a + 20 | 0;
                            d = f[b >> 2] | 0;
                            if (d | 0) {
                                a = d;
                                c = b;
                                continue
                            }
                            b = a + 16 | 0;
                            d = f[b >> 2] | 0;
                            if (!d) break; else {
                                a = d;
                                c = b;
                            }
                        }
                        f[c >> 2] = 0;
                        b = a;
                    } else {
                        b = f[j + 8 >> 2] | 0;
                        f[b + 12 >> 2] = a;
                        f[a + 8 >> 2] = b;
                        b = a;
                    } while (0);
                    if (e | 0) {
                        a = f[j + 28 >> 2] | 0;
                        c = 4880 + (a << 2) | 0;
                        if ((j | 0) == (f[c >> 2] | 0)) {
                            f[c >> 2] = b;
                            if (!b) {
                                f[1145] = f[1145] & ~(1 << a);
                                break
                            }
                        } else {
                            f[e + 16 + (((f[e + 16 >> 2] | 0) != (j | 0) & 1) << 2) >> 2] = b;
                            if (!b) break
                        }
                        f[b + 24 >> 2] = e;
                        a = j + 16 | 0;
                        c = f[a >> 2] | 0;
                        if (c | 0) {
                            f[b + 16 >> 2] = c;
                            f[c + 24 >> 2] = b;
                        }
                        a = f[a + 4 >> 2] | 0;
                        if (a | 0) {
                            f[b + 20 >> 2] = a;
                            f[a + 24 >> 2] = b;
                        }
                    }
                } while (0);
                f[i + 4 >> 2] = g | 1;
                f[i + g >> 2] = g;
                if ((i | 0) == (f[1149] | 0)) {
                    f[1146] = g;
                    return
                } else c = g;
            } else {
                f[a >> 2] = d & -2;
                f[i + 4 >> 2] = c | 1;
                f[i + c >> 2] = c;
            }
            a = c >>> 3;
            if (c >>> 0 < 256) {
                b = 4616 + (a << 1 << 2) | 0;
                c = f[1144] | 0;
                a = 1 << a;
                if (!(c & a)) {
                    f[1144] = c | a;
                    a = b;
                    c = b + 8 | 0;
                } else {
                    c = b + 8 | 0;
                    a = f[c >> 2] | 0;
                }
                f[c >> 2] = i;
                f[a + 12 >> 2] = i;
                f[i + 8 >> 2] = a;
                f[i + 12 >> 2] = b;
                return
            }
            a = c >>> 8;
            if (a) if (c >>> 0 > 16777215) a = 31; else {
                h = (a + 1048320 | 0) >>> 16 & 8;
                j = a << h;
                g = (j + 520192 | 0) >>> 16 & 4;
                j = j << g;
                a = (j + 245760 | 0) >>> 16 & 2;
                a = 14 - (g | h | a) + (j << a >>> 15) | 0;
                a = c >>> (a + 7 | 0) & 1 | a << 1;
            } else a = 0;
            e = 4880 + (a << 2) | 0;
            f[i + 28 >> 2] = a;
            f[i + 20 >> 2] = 0;
            f[i + 16 >> 2] = 0;
            b = f[1145] | 0;
            d = 1 << a;
            if (!(b & d)) {
                f[1145] = b | d;
                f[e >> 2] = i;
                f[i + 24 >> 2] = e;
                f[i + 12 >> 2] = i;
                f[i + 8 >> 2] = i;
                return
            }
            b = c << ((a | 0) == 31 ? 0 : 25 - (a >>> 1) | 0);
            d = f[e >> 2] | 0;
            while (1) {
                if ((f[d + 4 >> 2] & -8 | 0) == (c | 0)) {
                    a = 69;
                    break
                }
                e = d + 16 + (b >>> 31 << 2) | 0;
                a = f[e >> 2] | 0;
                if (!a) {
                    a = 68;
                    break
                } else {
                    b = b << 1;
                    d = a;
                }
            }
            if ((a | 0) == 68) {
                f[e >> 2] = i;
                f[i + 24 >> 2] = d;
                f[i + 12 >> 2] = i;
                f[i + 8 >> 2] = i;
            } else if ((a | 0) == 69) {
                h = d + 8 | 0;
                j = f[h >> 2] | 0;
                f[j + 12 >> 2] = i;
                f[h >> 2] = i;
                f[i + 8 >> 2] = j;
                f[i + 12 >> 2] = d;
                f[i + 24 >> 2] = 0;
            }
        }

        function Wa(a) {
            a = a | 0;
            var c = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0,
                v = 0, w = 0, x = 0, y = 0, z = 0, A = 0, B = 0, C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0, J = 0,
                K = 0, L = 0, M = 0, N = 0, O = 0, P = 0, Q = 0, R = 0, S = 0, T = 0, U = 0;
            S = u;
            u = u + 2416 | 0;
            k = S;
            j = S + 1904 | 0;
            R = S + 1880 | 0;
            O = S + 980 | 0;
            P = S + 80 | 0;
            Q = S + 16 | 0;
            e = f[a + 88 >> 2] | 0;
            M = (h[e + 63 >> 0] | 0) << 8 | (h[e + 64 >> 0] | 0);
            N = a + 92 | 0;
            c = (f[a + 4 >> 2] | 0) + ((h[e + 58 >> 0] | 0) << 8 | (h[e + 57 >> 0] | 0) << 16 | (h[e + 59 >> 0] | 0)) | 0;
            e = (h[e + 61 >> 0] | 0) << 8 | (h[e + 60 >> 0] | 0) << 16 | (h[e + 62 >> 0] | 0);
            if (!e) {
                R = 0;
                u = S;
                return R | 0
            }
            f[N >> 2] = c;
            f[a + 96 >> 2] = c;
            f[a + 104 >> 2] = e;
            f[a + 100 >> 2] = c + e;
            f[a + 108 >> 2] = 0;
            f[a + 112 >> 2] = 0;
            f[R + 20 >> 2] = 0;
            f[R >> 2] = 0;
            f[R + 4 >> 2] = 0;
            f[R + 8 >> 2] = 0;
            f[R + 12 >> 2] = 0;
            b[R + 16 >> 0] = 0;
            if (Pa(N, R) | 0) {
                c = 0;
                e = -7;
                g = -7;
                while (1) {
                    f[O + (c << 2) >> 2] = g;
                    f[P + (c << 2) >> 2] = e;
                    i = (g | 0) > 6;
                    c = c + 1 | 0;
                    if ((c | 0) == 225) break; else {
                        e = (i & 1) + e | 0;
                        g = i ? -7 : g + 1 | 0;
                    }
                }
                c = Q;
                e = c + 64 | 0;
                do {
                    f[c >> 2] = 0;
                    c = c + 4 | 0;
                } while ((c | 0) < (e | 0));
                i = a + 284 | 0;
                e = M * 3 | 0;
                g = a + 288 | 0;
                c = f[g >> 2] | 0;
                a:do if ((c | 0) == (e | 0)) l = 13; else {
                    if (c >>> 0 <= e >>> 0) {
                        do if ((f[a + 292 >> 2] | 0) >>> 0 < e >>> 0) if (fb(i, e, (c + 1 | 0) == (e | 0), 2, 0) | 0) {
                            c = f[g >> 2] | 0;
                            break
                        } else {
                            b[a + 296 >> 0] = 1;
                            c = 0;
                            break a
                        } while (0);
                        Ib((f[i >> 2] | 0) + (c << 1) | 0, 0, e - c << 1 | 0) | 0;
                    }
                    f[g >> 2] = e;
                    l = 13;
                } while (0);
                do if ((l | 0) == 13) {
                    if (!M) {
                        f[k >> 2] = 866;
                        f[k + 4 >> 2] = 910;
                        f[k + 8 >> 2] = 1497;
                        vc(j, 812, k) | 0;
                        Ub(j) | 0;
                        c = 1;
                        break
                    }
                    x = Q + 4 | 0;
                    y = Q + 8 | 0;
                    z = Q + 12 | 0;
                    A = Q + 16 | 0;
                    B = Q + 20 | 0;
                    C = Q + 24 | 0;
                    D = Q + 28 | 0;
                    E = Q + 32 | 0;
                    F = Q + 36 | 0;
                    G = Q + 40 | 0;
                    H = Q + 44 | 0;
                    I = Q + 48 | 0;
                    J = Q + 52 | 0;
                    K = Q + 56 | 0;
                    L = Q + 60 | 0;
                    w = 0;
                    c = f[i >> 2] | 0;
                    e = f[Q >> 2] | 0;
                    g = f[x >> 2] | 0;
                    i = f[y >> 2] | 0;
                    a = f[z >> 2] | 0;
                    j = f[A >> 2] | 0;
                    k = f[B >> 2] | 0;
                    l = f[C >> 2] | 0;
                    m = f[D >> 2] | 0;
                    n = f[E >> 2] | 0;
                    o = f[F >> 2] | 0;
                    p = f[G >> 2] | 0;
                    q = f[H >> 2] | 0;
                    r = 0;
                    s = 0;
                    t = 0;
                    v = 0;
                    while (1) {
                        U = bb(N, R) | 0;
                        e = e + (f[O + (U << 2) >> 2] | 0) & 7;
                        g = g + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        i = i + (f[O + (U << 2) >> 2] | 0) & 7;
                        a = a + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        j = j + (f[O + (U << 2) >> 2] | 0) & 7;
                        k = k + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        l = l + (f[O + (U << 2) >> 2] | 0) & 7;
                        m = m + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        n = n + (f[O + (U << 2) >> 2] | 0) & 7;
                        o = o + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        p = p + (f[O + (U << 2) >> 2] | 0) & 7;
                        q = q + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        r = r + (f[O + (U << 2) >> 2] | 0) & 7;
                        s = s + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = bb(N, R) | 0;
                        t = t + (f[O + (U << 2) >> 2] | 0) & 7;
                        v = v + (f[P + (U << 2) >> 2] | 0) & 7;
                        U = h[1445 + k >> 0] | 0;
                        d[c >> 1] = (h[1445 + g >> 0] | 0) << 3 | (h[1445 + e >> 0] | 0) | (h[1445 + i >> 0] | 0) << 6 | (h[1445 + a >> 0] | 0) << 9 | (h[1445 + j >> 0] | 0) << 12 | U << 15;
                        T = h[1445 + p >> 0] | 0;
                        d[c + 2 >> 1] = (h[1445 + l >> 0] | 0) << 2 | U >>> 1 | (h[1445 + m >> 0] | 0) << 5 | (h[1445 + n >> 0] | 0) << 8 | (h[1445 + o >> 0] | 0) << 11 | T << 14;
                        d[c + 4 >> 1] = (h[1445 + q >> 0] | 0) << 1 | T >>> 2 | (h[1445 + r >> 0] | 0) << 4 | (h[1445 + s >> 0] | 0) << 7 | (h[1445 + t >> 0] | 0) << 10 | (h[1445 + v >> 0] | 0) << 13;
                        w = w + 1 | 0;
                        if (w >>> 0 >= M >>> 0) break; else c = c + 6 | 0;
                    }
                    f[Q >> 2] = e;
                    f[x >> 2] = g;
                    f[y >> 2] = i;
                    f[z >> 2] = a;
                    f[A >> 2] = j;
                    f[B >> 2] = k;
                    f[C >> 2] = l;
                    f[D >> 2] = m;
                    f[E >> 2] = n;
                    f[F >> 2] = o;
                    f[G >> 2] = p;
                    f[H >> 2] = q;
                    f[I >> 2] = r;
                    f[J >> 2] = s;
                    f[K >> 2] = t;
                    f[L >> 2] = v;
                    c = 1;
                } while (0)
            } else c = 0;
            Cb(R);
            U = c;
            u = S;
            return U | 0
        }

        function Xa(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0,
                t = 0, v = 0, w = 0, x = 0, y = 0, z = 0, A = 0, B = 0, C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0,
                J = 0, K = 0, L = 0, M = 0, N = 0, O = 0, P = 0, Q = 0, R = 0;
            D = u;
            u = u + 1008 | 0;
            j = D;
            i = D + 496 | 0;
            C = D + 472 | 0;
            z = D + 276 | 0;
            A = D + 80 | 0;
            B = D + 16 | 0;
            d = f[a + 88 >> 2] | 0;
            x = (h[d + 47 >> 0] | 0) << 8 | (h[d + 48 >> 0] | 0);
            y = a + 92 | 0;
            c = (f[a + 4 >> 2] | 0) + ((h[d + 42 >> 0] | 0) << 8 | (h[d + 41 >> 0] | 0) << 16 | (h[d + 43 >> 0] | 0)) | 0;
            d = (h[d + 45 >> 0] | 0) << 8 | (h[d + 44 >> 0] | 0) << 16 | (h[d + 46 >> 0] | 0);
            if (!d) {
                C = 0;
                u = D;
                return C | 0
            }
            f[y >> 2] = c;
            f[a + 96 >> 2] = c;
            f[a + 104 >> 2] = d;
            f[a + 100 >> 2] = c + d;
            f[a + 108 >> 2] = 0;
            f[a + 112 >> 2] = 0;
            f[C + 20 >> 2] = 0;
            f[C >> 2] = 0;
            f[C + 4 >> 2] = 0;
            f[C + 8 >> 2] = 0;
            f[C + 12 >> 2] = 0;
            b[C + 16 >> 0] = 0;
            if (Pa(y, C) | 0) {
                c = 0;
                d = -3;
                e = -3;
                while (1) {
                    f[z + (c << 2) >> 2] = e;
                    f[A + (c << 2) >> 2] = d;
                    g = (e | 0) > 2;
                    c = c + 1 | 0;
                    if ((c | 0) == 49) break; else {
                        d = (g & 1) + d | 0;
                        e = g ? -3 : e + 1 | 0;
                    }
                }
                c = B;
                d = c + 64 | 0;
                do {
                    f[c >> 2] = 0;
                    c = c + 4 | 0;
                } while ((c | 0) < (d | 0));
                e = a + 252 | 0;
                d = a + 256 | 0;
                c = f[d >> 2] | 0;
                a:do if ((c | 0) == (x | 0)) k = 13; else {
                    if (c >>> 0 <= x >>> 0) {
                        do if ((f[a + 260 >> 2] | 0) >>> 0 < x >>> 0) if (fb(e, x, (c + 1 | 0) == (x | 0), 4, 0) | 0) {
                            c = f[d >> 2] | 0;
                            break
                        } else {
                            b[a + 264 >> 0] = 1;
                            c = 0;
                            break a
                        } while (0);
                        Ib((f[e >> 2] | 0) + (c << 2) | 0, 0, x - c << 2 | 0) | 0;
                    }
                    f[d >> 2] = x;
                    k = 13;
                } while (0);
                do if ((k | 0) == 13) {
                    if (!x) {
                        f[j >> 2] = 866;
                        f[j + 4 >> 2] = 910;
                        f[j + 8 >> 2] = 1497;
                        vc(i, 812, j) | 0;
                        Ub(i) | 0;
                        c = 1;
                        break
                    }
                    a = B + 4 | 0;
                    i = B + 8 | 0;
                    j = B + 12 | 0;
                    k = B + 16 | 0;
                    l = B + 20 | 0;
                    m = B + 24 | 0;
                    n = B + 28 | 0;
                    o = B + 32 | 0;
                    p = B + 36 | 0;
                    q = B + 40 | 0;
                    r = B + 44 | 0;
                    s = B + 48 | 0;
                    t = B + 52 | 0;
                    v = B + 56 | 0;
                    w = B + 60 | 0;
                    g = 0;
                    c = f[e >> 2] | 0;
                    d = f[a >> 2] | 0;
                    e = f[B >> 2] | 0;
                    while (1) {
                        Q = bb(y, C) | 0;
                        e = e + (f[z + (Q << 2) >> 2] | 0) & 3;
                        d = d + (f[A + (Q << 2) >> 2] | 0) & 3;
                        Q = bb(y, C) | 0;
                        R = (f[i >> 2] | 0) + (f[z + (Q << 2) >> 2] | 0) & 3;
                        f[i >> 2] = R;
                        Q = (f[j >> 2] | 0) + (f[A + (Q << 2) >> 2] | 0) & 3;
                        f[j >> 2] = Q;
                        O = bb(y, C) | 0;
                        P = (f[k >> 2] | 0) + (f[z + (O << 2) >> 2] | 0) & 3;
                        f[k >> 2] = P;
                        O = (f[l >> 2] | 0) + (f[A + (O << 2) >> 2] | 0) & 3;
                        f[l >> 2] = O;
                        M = bb(y, C) | 0;
                        N = (f[m >> 2] | 0) + (f[z + (M << 2) >> 2] | 0) & 3;
                        f[m >> 2] = N;
                        M = (f[n >> 2] | 0) + (f[A + (M << 2) >> 2] | 0) & 3;
                        f[n >> 2] = M;
                        K = bb(y, C) | 0;
                        L = (f[o >> 2] | 0) + (f[z + (K << 2) >> 2] | 0) & 3;
                        f[o >> 2] = L;
                        K = (f[p >> 2] | 0) + (f[A + (K << 2) >> 2] | 0) & 3;
                        f[p >> 2] = K;
                        I = bb(y, C) | 0;
                        J = (f[q >> 2] | 0) + (f[z + (I << 2) >> 2] | 0) & 3;
                        f[q >> 2] = J;
                        I = (f[r >> 2] | 0) + (f[A + (I << 2) >> 2] | 0) & 3;
                        f[r >> 2] = I;
                        G = bb(y, C) | 0;
                        H = (f[s >> 2] | 0) + (f[z + (G << 2) >> 2] | 0) & 3;
                        f[s >> 2] = H;
                        G = (f[t >> 2] | 0) + (f[A + (G << 2) >> 2] | 0) & 3;
                        f[t >> 2] = G;
                        E = bb(y, C) | 0;
                        F = (f[v >> 2] | 0) + (f[z + (E << 2) >> 2] | 0) & 3;
                        f[v >> 2] = F;
                        E = (f[w >> 2] | 0) + (f[A + (E << 2) >> 2] | 0) & 3;
                        f[w >> 2] = E;
                        f[c >> 2] = (h[1441 + d >> 0] | 0) << 2 | (h[1441 + e >> 0] | 0) | (h[1441 + R >> 0] | 0) << 4 | (h[1441 + Q >> 0] | 0) << 6 | (h[1441 + P >> 0] | 0) << 8 | (h[1441 + O >> 0] | 0) << 10 | (h[1441 + N >> 0] | 0) << 12 | (h[1441 + M >> 0] | 0) << 14 | (h[1441 + L >> 0] | 0) << 16 | (h[1441 + K >> 0] | 0) << 18 | (h[1441 + J >> 0] | 0) << 20 | (h[1441 + I >> 0] | 0) << 22 | (h[1441 + H >> 0] | 0) << 24 | (h[1441 + G >> 0] | 0) << 26 | (h[1441 + F >> 0] | 0) << 28 | (h[1441 + E >> 0] | 0) << 30;
                        g = g + 1 | 0;
                        if (g >>> 0 >= x >>> 0) break; else c = c + 4 | 0;
                    }
                    f[B >> 2] = e;
                    f[a >> 2] = d;
                    c = 1;
                } while (0)
            } else c = 0;
            Cb(C);
            R = c;
            u = D;
            return R | 0
        }

        function Ya(a, c, d, e, g, i, k, l) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            i = i | 0;
            k = k | 0;
            l = l | 0;
            var m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, v = 0, w = 0, x = 0, y = 0, z = 0, A = 0, B = 0,
                C = 0, D = 0, E = 0, F = 0, G = 0, H = 0, I = 0, J = 0, K = 0, L = 0, M = 0, N = 0, O = 0, P = 0, Q = 0,
                R = 0, S = 0, T = 0, U = 0, V = 0, W = 0, X = 0, Y = 0, Z = 0, _ = 0, $ = 0, aa = 0;
            aa = u;
            u = u + 592 | 0;
            Z = aa + 48 | 0;
            $ = aa + 32 | 0;
            _ = aa + 16 | 0;
            Y = aa;
            W = aa + 80 | 0;
            X = aa + 64 | 0;
            I = a + 272 | 0;
            J = f[I >> 2] | 0;
            V = f[a + 88 >> 2] | 0;
            K = (h[V + 63 >> 0] | 0) << 8 | (h[V + 64 >> 0] | 0);
            V = b[V + 17 >> 0] | 0;
            L = V & 255;
            if (!(V << 24 >> 24)) {
                u = aa;
                return 1
            }
            M = (l | 0) == 0;
            N = k + -1 | 0;
            O = N << 4;
            P = l + -1 | 0;
            Q = e << 1;
            R = a + 92 | 0;
            S = a + 116 | 0;
            T = a + 164 | 0;
            U = a + 268 | 0;
            V = a + 212 | 0;
            H = (g & 1 | 0) == 0;
            G = (i & 1 | 0) == 0;
            F = a + 288 | 0;
            E = a + 284 | 0;
            D = 0;
            g = 0;
            d = 0;
            i = 1;
            do {
                if (!M) {
                    B = f[c + (D << 2) >> 2] | 0;
                    C = 0;
                    while (1) {
                        A = C & 1;
                        m = (A | 0) == 0;
                        z = (A << 5 ^ 32) + -16 | 0;
                        A = (A << 1 ^ 2) + -1 | 0;
                        x = m ? k : -1;
                        a = m ? 0 : N;
                        if ((a | 0) != (x | 0)) {
                            y = G | (C | 0) != (P | 0);
                            w = m ? B : B + O | 0;
                            while (1) {
                                if ((i | 0) == 1) i = bb(R, S) | 0 | 512;
                                v = i & 7;
                                i = i >>> 3;
                                n = h[1539 + v >> 0] | 0;
                                m = 0;
                                do {
                                    r = (bb(R, T) | 0) + d | 0;
                                    s = r - J | 0;
                                    t = s >> 31;
                                    d = t & r | s & ~t;
                                    if ((f[I >> 2] | 0) >>> 0 <= d >>> 0) {
                                        f[Y >> 2] = 866;
                                        f[Y + 4 >> 2] = 910;
                                        f[Y + 8 >> 2] = 1497;
                                        vc(W, 812, Y) | 0;
                                        Ub(W) | 0;
                                    }
                                    f[X + (m << 2) >> 2] = j[(f[U >> 2] | 0) + (d << 1) >> 1];
                                    m = m + 1 | 0;
                                } while (m >>> 0 < n >>> 0);
                                t = H | (a | 0) != (N | 0);
                                r = 0;
                                s = w;
                                while (1) {
                                    q = y | (r | 0) == 0;
                                    n = r << 1;
                                    m = (bb(R, V) | 0) + g | 0;
                                    o = m - K | 0;
                                    p = o >> 31;
                                    p = p & m | o & ~p;
                                    if (t) {
                                        if (q) {
                                            g = h[1547 + (v << 2) + n >> 0] | 0;
                                            m = p * 3 | 0;
                                            if ((f[F >> 2] | 0) >>> 0 <= m >>> 0) {
                                                f[_ >> 2] = 866;
                                                f[_ + 4 >> 2] = 910;
                                                f[_ + 8 >> 2] = 1497;
                                                vc(W, 812, _) | 0;
                                                Ub(W) | 0;
                                            }
                                            o = (f[E >> 2] | 0) + (m << 1) | 0;
                                            f[s >> 2] = (j[o >> 1] | 0) << 16 | f[X + (g << 2) >> 2];
                                            f[s + 4 >> 2] = (j[o + 4 >> 1] | 0) << 16 | (j[o + 2 >> 1] | 0);
                                        }
                                        o = s + 8 | 0;
                                        m = (bb(R, V) | 0) + p | 0;
                                        p = m - K | 0;
                                        g = p >> 31;
                                        g = g & m | p & ~g;
                                        if (q) {
                                            m = h[(n | 1) + (1547 + (v << 2)) >> 0] | 0;
                                            n = g * 3 | 0;
                                            if ((f[F >> 2] | 0) >>> 0 <= n >>> 0) {
                                                f[Z >> 2] = 866;
                                                f[Z + 4 >> 2] = 910;
                                                f[Z + 8 >> 2] = 1497;
                                                vc(W, 812, Z) | 0;
                                                Ub(W) | 0;
                                            }
                                            q = (f[E >> 2] | 0) + (n << 1) | 0;
                                            f[o >> 2] = (j[q >> 1] | 0) << 16 | f[X + (m << 2) >> 2];
                                            f[s + 12 >> 2] = (j[q + 4 >> 1] | 0) << 16 | (j[q + 2 >> 1] | 0);
                                        }
                                    } else {
                                        if (q) {
                                            g = h[1547 + (v << 2) + n >> 0] | 0;
                                            m = p * 3 | 0;
                                            if ((f[F >> 2] | 0) >>> 0 <= m >>> 0) {
                                                f[$ >> 2] = 866;
                                                f[$ + 4 >> 2] = 910;
                                                f[$ + 8 >> 2] = 1497;
                                                vc(W, 812, $) | 0;
                                                Ub(W) | 0;
                                            }
                                            q = (f[E >> 2] | 0) + (m << 1) | 0;
                                            f[s >> 2] = (j[q >> 1] | 0) << 16 | f[X + (g << 2) >> 2];
                                            f[s + 4 >> 2] = (j[q + 4 >> 1] | 0) << 16 | (j[q + 2 >> 1] | 0);
                                        }
                                        p = (bb(R, V) | 0) + p | 0;
                                        q = p - K | 0;
                                        g = q >> 31;
                                        g = g & p | q & ~g;
                                    }
                                    r = r + 1 | 0;
                                    if ((r | 0) == 2) break; else s = s + e | 0;
                                }
                                a = A + a | 0;
                                if ((a | 0) == (x | 0)) break; else w = w + z | 0;
                            }
                        }
                        C = C + 1 | 0;
                        if ((C | 0) == (l | 0)) break; else B = B + Q | 0;
                    }
                }
                D = D + 1 | 0;
            } while ((D | 0) != (L | 0));
            u = aa;
            return 1
        }

        function Za(a, b, c, d, e) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0;
            l = a;
            j = b;
            k = j;
            h = c;
            n = d;
            i = n;
            if (!k) {
                g = (e | 0) != 0;
                if (!i) {
                    if (g) {
                        f[e >> 2] = (l >>> 0) % (h >>> 0);
                        f[e + 4 >> 2] = 0;
                    }
                    n = 0;
                    e = (l >>> 0) / (h >>> 0) >>> 0;
                    return (I = n, e) | 0
                } else {
                    if (!g) {
                        n = 0;
                        e = 0;
                        return (I = n, e) | 0
                    }
                    f[e >> 2] = a | 0;
                    f[e + 4 >> 2] = b & 0;
                    n = 0;
                    e = 0;
                    return (I = n, e) | 0
                }
            }
            g = (i | 0) == 0;
            do if (h) {
                if (!g) {
                    g = (_(i | 0) | 0) - (_(k | 0) | 0) | 0;
                    if (g >>> 0 <= 31) {
                        m = g + 1 | 0;
                        i = 31 - g | 0;
                        b = g - 31 >> 31;
                        h = m;
                        a = l >>> (m >>> 0) & b | k << i;
                        b = k >>> (m >>> 0) & b;
                        g = 0;
                        i = l << i;
                        break
                    }
                    if (!e) {
                        n = 0;
                        e = 0;
                        return (I = n, e) | 0
                    }
                    f[e >> 2] = a | 0;
                    f[e + 4 >> 2] = j | b & 0;
                    n = 0;
                    e = 0;
                    return (I = n, e) | 0
                }
                g = h - 1 | 0;
                if (g & h | 0) {
                    i = (_(h | 0) | 0) + 33 - (_(k | 0) | 0) | 0;
                    p = 64 - i | 0;
                    m = 32 - i | 0;
                    j = m >> 31;
                    o = i - 32 | 0;
                    b = o >> 31;
                    h = i;
                    a = m - 1 >> 31 & k >>> (o >>> 0) | (k << m | l >>> (i >>> 0)) & b;
                    b = b & k >>> (i >>> 0);
                    g = l << p & j;
                    i = (k << p | l >>> (o >>> 0)) & j | l << m & i - 33 >> 31;
                    break
                }
                if (e | 0) {
                    f[e >> 2] = g & l;
                    f[e + 4 >> 2] = 0;
                }
                if ((h | 0) == 1) {
                    o = j | b & 0;
                    p = a | 0 | 0;
                    return (I = o, p) | 0
                } else {
                    p = ic(h | 0) | 0;
                    o = k >>> (p >>> 0) | 0;
                    p = k << 32 - p | l >>> (p >>> 0) | 0;
                    return (I = o, p) | 0
                }
            } else {
                if (g) {
                    if (e | 0) {
                        f[e >> 2] = (k >>> 0) % (h >>> 0);
                        f[e + 4 >> 2] = 0;
                    }
                    o = 0;
                    p = (k >>> 0) / (h >>> 0) >>> 0;
                    return (I = o, p) | 0
                }
                if (!l) {
                    if (e | 0) {
                        f[e >> 2] = 0;
                        f[e + 4 >> 2] = (k >>> 0) % (i >>> 0);
                    }
                    o = 0;
                    p = (k >>> 0) / (i >>> 0) >>> 0;
                    return (I = o, p) | 0
                }
                g = i - 1 | 0;
                if (!(g & i)) {
                    if (e | 0) {
                        f[e >> 2] = a | 0;
                        f[e + 4 >> 2] = g & k | b & 0;
                    }
                    o = 0;
                    p = k >>> ((ic(i | 0) | 0) >>> 0);
                    return (I = o, p) | 0
                }
                g = (_(i | 0) | 0) - (_(k | 0) | 0) | 0;
                if (g >>> 0 <= 30) {
                    b = g + 1 | 0;
                    i = 31 - g | 0;
                    h = b;
                    a = k << i | l >>> (b >>> 0);
                    b = k >>> (b >>> 0);
                    g = 0;
                    i = l << i;
                    break
                }
                if (!e) {
                    o = 0;
                    p = 0;
                    return (I = o, p) | 0
                }
                f[e >> 2] = a | 0;
                f[e + 4 >> 2] = j | b & 0;
                o = 0;
                p = 0;
                return (I = o, p) | 0
            } while (0);
            if (!h) {
                k = i;
                j = 0;
                i = 0;
            } else {
                m = c | 0 | 0;
                l = n | d & 0;
                k = Gc(m | 0, l | 0, -1, -1) | 0;
                c = I;
                j = i;
                i = 0;
                do {
                    d = j;
                    j = g >>> 31 | j << 1;
                    g = i | g << 1;
                    d = a << 1 | d >>> 31 | 0;
                    n = a >>> 31 | b << 1 | 0;
                    Cc(k | 0, c | 0, d | 0, n | 0) | 0;
                    p = I;
                    o = p >> 31 | ((p | 0) < 0 ? -1 : 0) << 1;
                    i = o & 1;
                    a = Cc(d | 0, n | 0, o & m | 0, (((p | 0) < 0 ? -1 : 0) >> 31 | ((p | 0) < 0 ? -1 : 0) << 1) & l | 0) | 0;
                    b = I;
                    h = h - 1 | 0;
                } while ((h | 0) != 0);
                k = j;
                j = 0;
            }
            h = 0;
            if (e | 0) {
                f[e >> 2] = a;
                f[e + 4 >> 2] = b;
            }
            o = (g | 0) >>> 31 | (k | h) << 1 | (h << 1 | g >>> 31) & 0 | j;
            p = (g << 1 | 0 >>> 31) & -2 | i;
            return (I = o, p) | 0
        }

        function _a(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0;
            m = a + 4 | 0;
            l = f[m >> 2] | 0;
            c = l & -8;
            i = a + c | 0;
            if (!(l & 3)) {
                if (b >>> 0 < 256) {
                    a = 0;
                    return a | 0
                }
                if (c >>> 0 >= (b + 4 | 0) >>> 0 ? (c - b | 0) >>> 0 <= f[1264] << 1 >>> 0 : 0) return a | 0;
                a = 0;
                return a | 0
            }
            if (c >>> 0 >= b >>> 0) {
                c = c - b | 0;
                if (c >>> 0 <= 15) return a | 0;
                k = a + b | 0;
                f[m >> 2] = l & 1 | b | 2;
                f[k + 4 >> 2] = c | 3;
                m = k + c + 4 | 0;
                f[m >> 2] = f[m >> 2] | 1;
                Va(k, c);
                return a | 0
            }
            if ((i | 0) == (f[1150] | 0)) {
                k = (f[1147] | 0) + c | 0;
                c = k - b | 0;
                d = a + b | 0;
                if (k >>> 0 <= b >>> 0) {
                    a = 0;
                    return a | 0
                }
                f[m >> 2] = l & 1 | b | 2;
                f[d + 4 >> 2] = c | 1;
                f[1150] = d;
                f[1147] = c;
                return a | 0
            }
            if ((i | 0) == (f[1149] | 0)) {
                e = (f[1146] | 0) + c | 0;
                if (e >>> 0 < b >>> 0) {
                    a = 0;
                    return a | 0
                }
                c = e - b | 0;
                d = l & 1;
                if (c >>> 0 > 15) {
                    l = a + b | 0;
                    k = l + c | 0;
                    f[m >> 2] = d | b | 2;
                    f[l + 4 >> 2] = c | 1;
                    f[k >> 2] = c;
                    d = k + 4 | 0;
                    f[d >> 2] = f[d >> 2] & -2;
                    d = l;
                } else {
                    f[m >> 2] = d | e | 2;
                    d = a + e + 4 | 0;
                    f[d >> 2] = f[d >> 2] | 1;
                    d = 0;
                    c = 0;
                }
                f[1146] = c;
                f[1149] = d;
                return a | 0
            }
            d = f[i + 4 >> 2] | 0;
            if (d & 2 | 0) {
                a = 0;
                return a | 0
            }
            j = (d & -8) + c | 0;
            if (j >>> 0 < b >>> 0) {
                a = 0;
                return a | 0
            }
            k = j - b | 0;
            e = d >>> 3;
            do if (d >>> 0 < 256) {
                d = f[i + 8 >> 2] | 0;
                c = f[i + 12 >> 2] | 0;
                if ((c | 0) == (d | 0)) {
                    f[1144] = f[1144] & ~(1 << e);
                    break
                } else {
                    f[d + 12 >> 2] = c;
                    f[c + 8 >> 2] = d;
                    break
                }
            } else {
                h = f[i + 24 >> 2] | 0;
                c = f[i + 12 >> 2] | 0;
                do if ((c | 0) == (i | 0)) {
                    e = i + 16 | 0;
                    d = e + 4 | 0;
                    c = f[d >> 2] | 0;
                    if (!c) {
                        c = f[e >> 2] | 0;
                        if (!c) {
                            e = 0;
                            break
                        } else g = e;
                    } else g = d;
                    while (1) {
                        e = c + 20 | 0;
                        d = f[e >> 2] | 0;
                        if (d | 0) {
                            c = d;
                            g = e;
                            continue
                        }
                        d = c + 16 | 0;
                        e = f[d >> 2] | 0;
                        if (!e) break; else {
                            c = e;
                            g = d;
                        }
                    }
                    f[g >> 2] = 0;
                    e = c;
                } else {
                    e = f[i + 8 >> 2] | 0;
                    f[e + 12 >> 2] = c;
                    f[c + 8 >> 2] = e;
                    e = c;
                } while (0);
                if (h | 0) {
                    c = f[i + 28 >> 2] | 0;
                    d = 4880 + (c << 2) | 0;
                    if ((i | 0) == (f[d >> 2] | 0)) {
                        f[d >> 2] = e;
                        if (!e) {
                            f[1145] = f[1145] & ~(1 << c);
                            break
                        }
                    } else {
                        f[h + 16 + (((f[h + 16 >> 2] | 0) != (i | 0) & 1) << 2) >> 2] = e;
                        if (!e) break
                    }
                    f[e + 24 >> 2] = h;
                    c = i + 16 | 0;
                    d = f[c >> 2] | 0;
                    if (d | 0) {
                        f[e + 16 >> 2] = d;
                        f[d + 24 >> 2] = e;
                    }
                    c = f[c + 4 >> 2] | 0;
                    if (c | 0) {
                        f[e + 20 >> 2] = c;
                        f[c + 24 >> 2] = e;
                    }
                }
            } while (0);
            c = l & 1;
            if (k >>> 0 < 16) {
                f[m >> 2] = j | c | 2;
                m = a + j + 4 | 0;
                f[m >> 2] = f[m >> 2] | 1;
                return a | 0
            } else {
                l = a + b | 0;
                f[m >> 2] = c | b | 2;
                f[l + 4 >> 2] = k | 3;
                m = l + k + 4 | 0;
                f[m >> 2] = f[m >> 2] | 1;
                Va(l, k);
                return a | 0
            }
        }

        function $a(a, b, c, d, e, g) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            var h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0;
            p = u;
            u = u + 592 | 0;
            o = p + 56 | 0;
            j = p + 40 | 0;
            m = p + 72 | 0;
            l = p;
            n = p + 68 | 0;
            f[l >> 2] = 40;
            tb(a, b, l) | 0;
            h = (f[l + 4 >> 2] | 0) >>> e;
            i = (f[l + 8 >> 2] | 0) >>> e;
            l = l + 32 | 0;
            d = f[l + 4 >> 2] | 0;
            do switch (f[l >> 2] | 0) {
                case 0: {
                    if (!d) l = 8; else k = 14;
                    break
                }
                case 1: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 2: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 3: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 4: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 5: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 6: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 7: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 8: {
                    if (!d) k = 13; else k = 14;
                    break
                }
                case 9: {
                    if (!d) l = 8; else k = 14;
                    break
                }
                case 10: {
                    if (!d) l = 8; else k = 14;
                    break
                }
                default:
                    k = 14;
            } while (0);
            if ((k | 0) == 13) l = 16; else if ((k | 0) == 14) {
                f[j >> 2] = 866;
                f[j + 4 >> 2] = 2672;
                f[j + 8 >> 2] = 1251;
                vc(m, 812, j) | 0;
                Ub(m) | 0;
                l = 0;
            }
            f[n >> 2] = c;
            k = gb(a, b) | 0;
            b = g + e | 0;
            do if (b >>> 0 > e >>> 0) {
                if (!k) {
                    d = c;
                    while (1) {
                        d = d + (X(X((h + 3 | 0) >>> 2, l) | 0, (i + 3 | 0) >>> 2) | 0) | 0;
                        e = e + 1 | 0;
                        if ((e | 0) == (b | 0)) break; else {
                            i = i >>> 1;
                            h = h >>> 1;
                        }
                    }
                    f[n >> 2] = d;
                    break
                } else {
                    a = i;
                    d = c;
                }
                while (1) {
                    i = X((h + 3 | 0) >>> 2, l) | 0;
                    j = X(i, (a + 3 | 0) >>> 2) | 0;
                    if (!(e >>> 0 > 15 | j >>> 0 < 8) ? (f[k >> 2] | 0) == 519686845 : 0) {
                        wb(k, n, j, i, e) | 0;
                        d = f[n >> 2] | 0;
                    }
                    d = d + j | 0;
                    f[n >> 2] = d;
                    e = e + 1 | 0;
                    if ((e | 0) == (b | 0)) break; else {
                        a = a >>> 1;
                        h = h >>> 1;
                    }
                }
            } while (0);
            if (!k) {
                u = p;
                return
            }
            if ((f[k >> 2] | 0) != 519686845) {
                u = p;
                return
            }
            cb(k);
            if (!(k & 7)) {
                Nb(k, 0, 0, 1, 0) | 0;
                u = p;
            } else {
                f[o >> 2] = 866;
                f[o + 4 >> 2] = 2506;
                f[o + 8 >> 2] = 1232;
                vc(m, 812, o) | 0;
                Ub(m) | 0;
                u = p;
            }
        }

        function ab(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0;
            q = u;
            u = u + 576 | 0;
            j = q;
            g = q + 64 | 0;
            p = q + 16 | 0;
            e = a + 88 | 0;
            c = f[e >> 2] | 0;
            o = (h[c + 39 >> 0] | 0) << 8 | (h[c + 40 >> 0] | 0);
            m = a + 236 | 0;
            i = a + 240 | 0;
            d = f[i >> 2] | 0;
            if ((d | 0) != (o | 0)) {
                if (d >>> 0 <= o >>> 0) {
                    do if ((f[a + 244 >> 2] | 0) >>> 0 < o >>> 0) {
                        if (fb(m, o, (d + 1 | 0) == (o | 0), 4, 0) | 0) {
                            c = f[i >> 2] | 0;
                            break
                        }
                        b[a + 248 >> 0] = 1;
                        p = 0;
                        u = q;
                        return p | 0
                    } else c = d; while (0);
                    Ib((f[m >> 2] | 0) + (c << 2) | 0, 0, o - c << 2 | 0) | 0;
                    c = f[e >> 2] | 0;
                }
                f[i >> 2] = o;
            }
            n = a + 92 | 0;
            d = (f[a + 4 >> 2] | 0) + ((h[c + 34 >> 0] | 0) << 8 | (h[c + 33 >> 0] | 0) << 16 | (h[c + 35 >> 0] | 0)) | 0;
            c = (h[c + 37 >> 0] | 0) << 8 | (h[c + 36 >> 0] | 0) << 16 | (h[c + 38 >> 0] | 0);
            if (!c) {
                p = 0;
                u = q;
                return p | 0
            }
            f[n >> 2] = d;
            f[a + 96 >> 2] = d;
            f[a + 104 >> 2] = c;
            f[a + 100 >> 2] = d + c;
            f[a + 108 >> 2] = 0;
            f[a + 112 >> 2] = 0;
            k = p + 20 | 0;
            f[p >> 2] = 0;
            f[p + 4 >> 2] = 0;
            f[p + 8 >> 2] = 0;
            f[p + 12 >> 2] = 0;
            b[p + 16 >> 0] = 0;
            l = p + 24 | 0;
            f[p + 44 >> 2] = 0;
            f[k >> 2] = 0;
            f[k + 4 >> 2] = 0;
            f[k + 8 >> 2] = 0;
            f[k + 12 >> 2] = 0;
            f[k + 16 >> 2] = 0;
            b[k + 20 >> 0] = 0;
            if (Pa(n, p) | 0 ? Pa(n, l) | 0 : 0) {
                if (!(f[i >> 2] | 0)) {
                    f[j >> 2] = 866;
                    f[j + 4 >> 2] = 910;
                    f[j + 8 >> 2] = 1497;
                    vc(g, 812, j) | 0;
                    Ub(g) | 0;
                }
                if (!o) c = 1; else {
                    j = 0;
                    k = 0;
                    d = f[m >> 2] | 0;
                    e = 0;
                    a = 0;
                    c = 0;
                    g = 0;
                    i = 0;
                    while (1) {
                        j = (bb(n, p) | 0) + j & 31;
                        i = (bb(n, l) | 0) + i & 63;
                        g = (bb(n, p) | 0) + g & 31;
                        c = (bb(n, p) | 0) + c | 0;
                        a = (bb(n, l) | 0) + a & 63;
                        e = (bb(n, p) | 0) + e & 31;
                        f[d >> 2] = i << 5 | j << 11 | g | c << 27 | a << 21 | e << 16;
                        k = k + 1 | 0;
                        if (k >>> 0 >= o >>> 0) {
                            c = 1;
                            break
                        } else {
                            d = d + 4 | 0;
                            c = c & 31;
                        }
                    }
                }
            } else c = 0;
            Cb(p + 24 | 0);
            Cb(p);
            p = c;
            u = q;
            return p | 0
        }

        function bb(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0, i = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0;
            s = u;
            u = u + 576 | 0;
            m = s + 48 | 0;
            o = s + 32 | 0;
            n = s + 16 | 0;
            l = s;
            q = s + 64 | 0;
            p = f[b + 20 >> 2] | 0;
            r = a + 20 | 0;
            k = f[r >> 2] | 0;
            if ((k | 0) < 24) {
                i = a + 4 | 0;
                c = f[i >> 2] | 0;
                e = f[a + 8 >> 2] | 0;
                d = c >>> 0 < e >>> 0;
                if ((k | 0) < 16) {
                    if (d) {
                        g = (h[c >> 0] | 0) << 8;
                        c = c + 1 | 0;
                    } else g = 0;
                    if (c >>> 0 < e >>> 0) {
                        e = h[c >> 0] | 0;
                        c = c + 1 | 0;
                    } else e = 0;
                    f[i >> 2] = c;
                    f[r >> 2] = k + 16;
                    d = 16;
                    c = e | g;
                } else {
                    if (d) {
                        f[i >> 2] = c + 1;
                        c = h[c >> 0] | 0;
                    } else c = 0;
                    f[r >> 2] = k + 8;
                    d = 24;
                }
                i = a + 16 | 0;
                e = f[i >> 2] | c << d - k;
                f[i >> 2] = e;
            } else {
                e = a + 16 | 0;
                i = e;
                e = f[e >> 2] | 0;
            }
            g = (e >>> 16) + 1 | 0;
            do if (g >>> 0 <= (f[p + 16 >> 2] | 0) >>> 0) {
                d = f[(f[p + 168 >> 2] | 0) + (e >>> (32 - (f[p + 8 >> 2] | 0) | 0) << 2) >> 2] | 0;
                if ((d | 0) == -1) {
                    f[l >> 2] = 866;
                    f[l + 4 >> 2] = 3253;
                    f[l + 8 >> 2] = 1393;
                    vc(q, 812, l) | 0;
                    Ub(q) | 0;
                }
                c = d & 65535;
                d = d >>> 16;
                if ((f[b + 8 >> 2] | 0) >>> 0 <= c >>> 0) {
                    f[n >> 2] = 866;
                    f[n + 4 >> 2] = 909;
                    f[n + 8 >> 2] = 1497;
                    vc(q, 812, n) | 0;
                    Ub(q) | 0;
                }
                if ((h[(f[b + 4 >> 2] | 0) + c >> 0] | 0 | 0) != (d | 0)) {
                    f[o >> 2] = 866;
                    f[o + 4 >> 2] = 3257;
                    f[o + 8 >> 2] = 1410;
                    vc(q, 812, o) | 0;
                    Ub(q) | 0;
                }
            } else {
                d = f[p + 20 >> 2] | 0;
                while (1) {
                    c = d + -1 | 0;
                    if (g >>> 0 > (f[p + 28 + (c << 2) >> 2] | 0) >>> 0) d = d + 1 | 0; else break
                }
                c = (e >>> (32 - d | 0)) + (f[p + 96 + (c << 2) >> 2] | 0) | 0;
                if (c >>> 0 < (f[b >> 2] | 0) >>> 0) {
                    c = j[(f[p + 176 >> 2] | 0) + (c << 1) >> 1] | 0;
                    break
                }
                f[m >> 2] = 866;
                f[m + 4 >> 2] = 3275;
                f[m + 8 >> 2] = 1348;
                vc(q, 812, m) | 0;
                Ub(q) | 0;
                r = 0;
                u = s;
                return r | 0
            } while (0);
            f[i >> 2] = f[i >> 2] << d;
            f[r >> 2] = (f[r >> 2] | 0) - d;
            r = c;
            u = s;
            return r | 0
        }

        function cb(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0, k = 0;
            k = u;
            u = u + 576 | 0;
            j = k + 48 | 0;
            h = k + 32 | 0;
            g = k + 16 | 0;
            e = k;
            i = k + 64 | 0;
            f[a >> 2] = 0;
            c = a + 284 | 0;
            d = f[c >> 2] | 0;
            if (d | 0) {
                if (!(d & 7)) Nb(d, 0, 0, 1, 0) | 0; else {
                    f[e >> 2] = 866;
                    f[e + 4 >> 2] = 2506;
                    f[e + 8 >> 2] = 1232;
                    vc(i, 812, e) | 0;
                    Ub(i) | 0;
                }
                f[c >> 2] = 0;
                f[a + 288 >> 2] = 0;
                f[a + 292 >> 2] = 0;
            }
            b[a + 296 >> 0] = 0;
            c = a + 268 | 0;
            d = f[c >> 2] | 0;
            if (d | 0) {
                if (!(d & 7)) Nb(d, 0, 0, 1, 0) | 0; else {
                    f[g >> 2] = 866;
                    f[g + 4 >> 2] = 2506;
                    f[g + 8 >> 2] = 1232;
                    vc(i, 812, g) | 0;
                    Ub(i) | 0;
                }
                f[c >> 2] = 0;
                f[a + 272 >> 2] = 0;
                f[a + 276 >> 2] = 0;
            }
            b[a + 280 >> 0] = 0;
            c = a + 252 | 0;
            d = f[c >> 2] | 0;
            if (d | 0) {
                if (!(d & 7)) Nb(d, 0, 0, 1, 0) | 0; else {
                    f[h >> 2] = 866;
                    f[h + 4 >> 2] = 2506;
                    f[h + 8 >> 2] = 1232;
                    vc(i, 812, h) | 0;
                    Ub(i) | 0;
                }
                f[c >> 2] = 0;
                f[a + 256 >> 2] = 0;
                f[a + 260 >> 2] = 0;
            }
            b[a + 264 >> 0] = 0;
            c = a + 236 | 0;
            d = f[c >> 2] | 0;
            if (!d) {
                j = a + 248 | 0;
                b[j >> 0] = 0;
                j = a + 212 | 0;
                Cb(j);
                j = a + 188 | 0;
                Cb(j);
                j = a + 164 | 0;
                Cb(j);
                j = a + 140 | 0;
                Cb(j);
                j = a + 116 | 0;
                Cb(j);
                u = k;
                return
            }
            if (!(d & 7)) Nb(d, 0, 0, 1, 0) | 0; else {
                f[j >> 2] = 866;
                f[j + 4 >> 2] = 2506;
                f[j + 8 >> 2] = 1232;
                vc(i, 812, j) | 0;
                Ub(i) | 0;
            }
            f[c >> 2] = 0;
            f[a + 240 >> 2] = 0;
            f[a + 244 >> 2] = 0;
            j = a + 248 | 0;
            b[j >> 0] = 0;
            j = a + 212 | 0;
            Cb(j);
            j = a + 188 | 0;
            Cb(j);
            j = a + 164 | 0;
            Cb(j);
            j = a + 140 | 0;
            Cb(j);
            j = a + 116 | 0;
            Cb(j);
            u = k;
        }

        function db(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0.0;
            a:do if (b >>> 0 <= 20) do switch (b | 0) {
                case 9: {
                    d = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    b = f[d >> 2] | 0;
                    f[c >> 2] = d + 4;
                    f[a >> 2] = b;
                    break a
                }
                case 10: {
                    d = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    b = f[d >> 2] | 0;
                    f[c >> 2] = d + 4;
                    d = a;
                    f[d >> 2] = b;
                    f[d + 4 >> 2] = ((b | 0) < 0) << 31 >> 31;
                    break a
                }
                case 11: {
                    d = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    b = f[d >> 2] | 0;
                    f[c >> 2] = d + 4;
                    d = a;
                    f[d >> 2] = b;
                    f[d + 4 >> 2] = 0;
                    break a
                }
                case 12: {
                    d = (f[c >> 2] | 0) + (8 - 1) & ~(8 - 1);
                    b = d;
                    e = f[b >> 2] | 0;
                    b = f[b + 4 >> 2] | 0;
                    f[c >> 2] = d + 8;
                    d = a;
                    f[d >> 2] = e;
                    f[d + 4 >> 2] = b;
                    break a
                }
                case 13: {
                    e = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    d = f[e >> 2] | 0;
                    f[c >> 2] = e + 4;
                    d = (d & 65535) << 16 >> 16;
                    e = a;
                    f[e >> 2] = d;
                    f[e + 4 >> 2] = ((d | 0) < 0) << 31 >> 31;
                    break a
                }
                case 14: {
                    e = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    d = f[e >> 2] | 0;
                    f[c >> 2] = e + 4;
                    e = a;
                    f[e >> 2] = d & 65535;
                    f[e + 4 >> 2] = 0;
                    break a
                }
                case 15: {
                    e = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    d = f[e >> 2] | 0;
                    f[c >> 2] = e + 4;
                    d = (d & 255) << 24 >> 24;
                    e = a;
                    f[e >> 2] = d;
                    f[e + 4 >> 2] = ((d | 0) < 0) << 31 >> 31;
                    break a
                }
                case 16: {
                    e = (f[c >> 2] | 0) + (4 - 1) & ~(4 - 1);
                    d = f[e >> 2] | 0;
                    f[c >> 2] = e + 4;
                    e = a;
                    f[e >> 2] = d & 255;
                    f[e + 4 >> 2] = 0;
                    break a
                }
                case 17: {
                    e = (f[c >> 2] | 0) + (8 - 1) & ~(8 - 1);
                    g = +p[e >> 3];
                    f[c >> 2] = e + 8;
                    p[a >> 3] = g;
                    break a
                }
                case 18: {
                    e = (f[c >> 2] | 0) + (8 - 1) & ~(8 - 1);
                    g = +p[e >> 3];
                    f[c >> 2] = e + 8;
                    p[a >> 3] = g;
                    break a
                }
                default:
                    break a
            } while (0); while (0);
        }

        function eb(a) {
            a = a | 0;
            var c = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0;
            n = u;
            u = u + 560 | 0;
            i = n;
            g = n + 40 | 0;
            m = n + 16 | 0;
            e = f[a + 88 >> 2] | 0;
            k = (h[e + 55 >> 0] | 0) << 8 | (h[e + 56 >> 0] | 0);
            l = a + 92 | 0;
            c = (f[a + 4 >> 2] | 0) + ((h[e + 50 >> 0] | 0) << 8 | (h[e + 49 >> 0] | 0) << 16 | (h[e + 51 >> 0] | 0)) | 0;
            e = (h[e + 53 >> 0] | 0) << 8 | (h[e + 52 >> 0] | 0) << 16 | (h[e + 54 >> 0] | 0);
            if (!e) {
                m = 0;
                u = n;
                return m | 0
            }
            f[l >> 2] = c;
            f[a + 96 >> 2] = c;
            f[a + 104 >> 2] = e;
            f[a + 100 >> 2] = c + e;
            f[a + 108 >> 2] = 0;
            f[a + 112 >> 2] = 0;
            f[m + 20 >> 2] = 0;
            f[m >> 2] = 0;
            f[m + 4 >> 2] = 0;
            f[m + 8 >> 2] = 0;
            f[m + 12 >> 2] = 0;
            b[m + 16 >> 0] = 0;
            a:do if (Pa(l, m) | 0) {
                j = a + 268 | 0;
                e = a + 272 | 0;
                c = f[e >> 2] | 0;
                if ((c | 0) != (k | 0)) {
                    if (c >>> 0 <= k >>> 0) {
                        do if ((f[a + 276 >> 2] | 0) >>> 0 < k >>> 0) if (fb(j, k, (c + 1 | 0) == (k | 0), 2, 0) | 0) {
                            c = f[e >> 2] | 0;
                            break
                        } else {
                            b[a + 280 >> 0] = 1;
                            c = 0;
                            break a
                        } while (0);
                        Ib((f[j >> 2] | 0) + (c << 1) | 0, 0, k - c << 1 | 0) | 0;
                    }
                    f[e >> 2] = k;
                }
                if (!k) {
                    f[i >> 2] = 866;
                    f[i + 4 >> 2] = 910;
                    f[i + 8 >> 2] = 1497;
                    vc(g, 812, i) | 0;
                    Ub(g) | 0;
                    c = 1;
                    break
                }
                e = 0;
                a = 0;
                g = 0;
                c = f[j >> 2] | 0;
                while (1) {
                    j = bb(l, m) | 0;
                    g = j + g & 255;
                    a = (bb(l, m) | 0) + a & 255;
                    d[c >> 1] = a << 8 | g;
                    e = e + 1 | 0;
                    if (e >>> 0 >= k >>> 0) {
                        c = 1;
                        break
                    } else c = c + 2 | 0;
                }
            } else c = 0; while (0);
            Cb(m);
            m = c;
            u = n;
            return m | 0
        }

        function fb(a, b, c, d, e) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0;
            p = u;
            u = u + 576 | 0;
            m = p + 48 | 0;
            j = p + 32 | 0;
            h = p + 16 | 0;
            g = p;
            l = p + 64 | 0;
            n = p + 60 | 0;
            k = a + 4 | 0;
            o = a + 8 | 0;
            if ((f[k >> 2] | 0) >>> 0 > (f[o >> 2] | 0) >>> 0) {
                f[g >> 2] = 866;
                f[g + 4 >> 2] = 2123;
                f[g + 8 >> 2] = 845;
                vc(l, 812, g) | 0;
                Ub(l) | 0;
            }
            if ((2147418112 / (d >>> 0) | 0) >>> 0 <= b >>> 0) {
                f[h >> 2] = 866;
                f[h + 4 >> 2] = 2124;
                f[h + 8 >> 2] = 885;
                vc(l, 812, h) | 0;
                Ub(l) | 0;
            }
            g = f[o >> 2] | 0;
            if (g >>> 0 >= b >>> 0) {
                o = 1;
                u = p;
                return o | 0
            }
            if (c ? (i = b + -1 | 0, (i & b | 0) != 0) : 0) {
                b = i >>> 16 | i;
                b = b >>> 8 | b;
                b = b >>> 4 | b;
                b = b >>> 2 | b;
                b = (b >>> 1 | b) + 1 | 0;
                if (!b) {
                    b = 0;
                    c = 10;
                } else c = 9;
            } else c = 9;
            if ((c | 0) == 9) if (b >>> 0 <= g >>> 0) c = 10;
            if ((c | 0) == 10) {
                f[j >> 2] = 866;
                f[j + 4 >> 2] = 2133;
                f[j + 8 >> 2] = 933;
                vc(l, 812, j) | 0;
                Ub(l) | 0;
            }
            i = X(b, d) | 0;
            if (!e) {
                g = Gb(f[a >> 2] | 0, i, n, 1) | 0;
                if (!g) b = 0; else {
                    f[a >> 2] = g;
                    c = 20;
                }
            } else {
                h = Db(i, n) | 0;
                if (!h) b = 0; else {
                    Ia[e & 0](h, f[a >> 2] | 0, f[k >> 2] | 0);
                    g = f[a >> 2] | 0;
                    do if (g | 0) if (!(g & 7)) {
                        Nb(g, 0, 0, 1, 0) | 0;
                        break
                    } else {
                        f[m >> 2] = 866;
                        f[m + 4 >> 2] = 2506;
                        f[m + 8 >> 2] = 1232;
                        vc(l, 812, m) | 0;
                        Ub(l) | 0;
                        break
                    } while (0);
                    f[a >> 2] = h;
                    c = 20;
                }
            }
            if ((c | 0) == 20) {
                g = f[n >> 2] | 0;
                if (g >>> 0 > i >>> 0) b = (g >>> 0) / (d >>> 0) | 0;
                f[o >> 2] = b;
                b = 1;
            }
            o = b;
            u = p;
            return o | 0
        }

        function gb(a, c) {
            a = a | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0;
            o = u;
            u = u + 528 | 0;
            m = o;
            j = o + 16 | 0;
            if ((a | 0) == 0 | c >>> 0 < 62) {
                p = 0;
                u = o;
                return p | 0
            }
            k = Db(300, 0) | 0;
            if (!k) {
                p = 0;
                u = o;
                return p | 0
            }
            f[k >> 2] = 519686845;
            f[k + 4 >> 2] = 0;
            f[k + 8 >> 2] = 0;
            l = k + 88 | 0;
            d = k + 136 | 0;
            e = k + 160 | 0;
            g = k + 184 | 0;
            h = k + 208 | 0;
            i = k + 232 | 0;
            n = k + 252 | 0;
            f[n >> 2] = 0;
            f[n + 4 >> 2] = 0;
            f[n + 8 >> 2] = 0;
            b[n + 12 >> 0] = 0;
            n = k + 268 | 0;
            f[n >> 2] = 0;
            f[n + 4 >> 2] = 0;
            f[n + 8 >> 2] = 0;
            b[n + 12 >> 0] = 0;
            n = k + 284 | 0;
            f[n >> 2] = 0;
            f[n + 4 >> 2] = 0;
            f[n + 8 >> 2] = 0;
            b[n + 12 >> 0] = 0;
            n = l;
            p = n + 44 | 0;
            do {
                f[n >> 2] = 0;
                n = n + 4 | 0;
            } while ((n | 0) < (p | 0));
            b[l + 44 >> 0] = 0;
            f[d >> 2] = 0;
            f[d + 4 >> 2] = 0;
            f[d + 8 >> 2] = 0;
            f[d + 12 >> 2] = 0;
            f[d + 16 >> 2] = 0;
            b[d + 20 >> 0] = 0;
            f[e >> 2] = 0;
            f[e + 4 >> 2] = 0;
            f[e + 8 >> 2] = 0;
            f[e + 12 >> 2] = 0;
            f[e + 16 >> 2] = 0;
            b[e + 20 >> 0] = 0;
            f[g >> 2] = 0;
            f[g + 4 >> 2] = 0;
            f[g + 8 >> 2] = 0;
            f[g + 12 >> 2] = 0;
            f[g + 16 >> 2] = 0;
            b[g + 20 >> 0] = 0;
            f[h >> 2] = 0;
            f[h + 4 >> 2] = 0;
            f[h + 8 >> 2] = 0;
            f[h + 12 >> 2] = 0;
            f[h + 16 >> 2] = 0;
            b[h + 20 >> 0] = 0;
            f[i >> 2] = 0;
            f[i + 4 >> 2] = 0;
            f[i + 8 >> 2] = 0;
            f[i + 12 >> 2] = 0;
            b[i + 16 >> 0] = 0;
            if (xb(k, a, c) | 0) {
                p = k;
                u = o;
                return p | 0
            }
            cb(k);
            if (!(k & 7)) {
                Nb(k, 0, 0, 1, 0) | 0;
                p = 0;
                u = o;
                return p | 0
            } else {
                f[m >> 2] = 866;
                f[m + 4 >> 2] = 2506;
                f[m + 8 >> 2] = 1232;
                vc(j, 812, m) | 0;
                Ub(j) | 0;
                p = 0;
                u = o;
                return p | 0
            }
        }

        function hb(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, h = 0, i = 0, j = 0;
            i = u;
            u = u + 576 | 0;
            g = i + 40 | 0;
            e = i + 56 | 0;
            j = i;
            f[j >> 2] = 40;
            tb(a, b, j) | 0;
            d = (((f[j + 4 >> 2] | 0) >>> c) + 3 | 0) >>> 2;
            b = (((f[j + 8 >> 2] | 0) >>> c) + 3 | 0) >>> 2;
            c = j + 32 | 0;
            a = f[c + 4 >> 2] | 0;
            do switch (f[c >> 2] | 0) {
                case 0: {
                    if (!a) a = 8; else h = 14;
                    break
                }
                case 1: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 2: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 3: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 4: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 5: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 6: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 7: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 8: {
                    if (!a) h = 13; else h = 14;
                    break
                }
                case 9: {
                    if (!a) a = 8; else h = 14;
                    break
                }
                case 10: {
                    if (!a) a = 8; else h = 14;
                    break
                }
                default:
                    h = 14;
            } while (0);
            if ((h | 0) == 13) a = 16; else if ((h | 0) == 14) {
                f[g >> 2] = 866;
                f[g + 4 >> 2] = 2672;
                f[g + 8 >> 2] = 1251;
                vc(e, 812, g) | 0;
                Ub(e) | 0;
                a = 0;
            }
            j = X(X(b, d) | 0, a) | 0;
            u = i;
            return j | 0
        }

        function ib(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0;
            e = u;
            u = u + 576 | 0;
            d = e + 40 | 0;
            c = e + 56 | 0;
            g = e;
            f[g >> 2] = 40;
            tb(a, b, g) | 0;
            b = g + 32 | 0;
            a = f[b + 4 >> 2] | 0;
            do switch (f[b >> 2] | 0) {
                case 0: {
                    if (!a) {
                        g = 8;
                        u = e;
                        return g | 0
                    } else a = 14;
                    break
                }
                case 1: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 2: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 3: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 4: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 5: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 6: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 7: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 8: {
                    if (!a) a = 13; else a = 14;
                    break
                }
                case 9: {
                    if (!a) {
                        g = 8;
                        u = e;
                        return g | 0
                    } else a = 14;
                    break
                }
                case 10: {
                    if (!a) {
                        g = 8;
                        u = e;
                        return g | 0
                    } else a = 14;
                    break
                }
                default:
                    a = 14;
            } while (0);
            if ((a | 0) == 13) {
                g = 16;
                u = e;
                return g | 0
            } else if ((a | 0) == 14) {
                f[d >> 2] = 866;
                f[d + 4 >> 2] = 2672;
                f[d + 8 >> 2] = 1251;
                vc(c, 812, d) | 0;
                Ub(c) | 0;
                g = 0;
                u = e;
                return g | 0
            }
            return 0
        }

        function jb(a, c, d, e, g, i, j) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            i = i | 0;
            j = j | 0;
            var k = 0, l = 0, m = 0, n = 0;
            n = f[a + 88 >> 2] | 0;
            l = (h[n + 12 >> 0] << 8 | h[n + 13 >> 0]) >>> j;
            m = (h[n + 14 >> 0] << 8 | h[n + 15 >> 0]) >>> j;
            l = ((l >>> 0 > 1 ? l : 1) + 3 | 0) >>> 2;
            m = ((m >>> 0 > 1 ? m : 1) + 3 | 0) >>> 2;
            n = n + 18 | 0;
            j = b[n >> 0] | 0;
            j = X(l, j << 24 >> 24 == 0 | j << 24 >> 24 == 9 ? 8 : 16) | 0;
            if (i) if ((i & 3 | 0) == 0 & j >>> 0 <= i >>> 0) j = i; else {
                g = 0;
                return g | 0
            }
            if ((X(j, m) | 0) >>> 0 > g >>> 0) {
                g = 0;
                return g | 0
            }
            i = (l + 1 | 0) >>> 1;
            k = (m + 1 | 0) >>> 1;
            if (!d) {
                g = 0;
                return g | 0
            }
            f[a + 92 >> 2] = c;
            f[a + 96 >> 2] = c;
            f[a + 104 >> 2] = d;
            f[a + 100 >> 2] = c + d;
            f[a + 108 >> 2] = 0;
            f[a + 112 >> 2] = 0;
            switch (b[n >> 0] | 0) {
                case 0: {
                    if (!(Ra(a, e, g, j, l, m, i, k) | 0)) {
                        g = 0;
                        return g | 0
                    }
                    break
                }
                case 4:
                case 6:
                case 5:
                case 3:
                case 2: {
                    if (!(Ua(a, e, g, j, l, m, i, k) | 0)) {
                        g = 0;
                        return g | 0
                    }
                    break
                }
                case 9: {
                    if (!(Ya(a, e, g, j, l, m, i, k) | 0)) {
                        g = 0;
                        return g | 0
                    }
                    break
                }
                case 8:
                case 7: {
                    if (!(Sa(a, e, g, j, l, m, i, k) | 0)) {
                        g = 0;
                        return g | 0
                    }
                    break
                }
                default: {
                    g = 0;
                    return g | 0
                }
            }
            g = 1;
            return g | 0
        }

        function kb(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0;
            if ((d | 0) >= 8192) return va(a | 0, c | 0, d | 0) | 0;
            h = a | 0;
            g = a + d | 0;
            if ((a & 3) == (c & 3)) {
                while (a & 3) {
                    if (!d) return h | 0;
                    b[a >> 0] = b[c >> 0] | 0;
                    a = a + 1 | 0;
                    c = c + 1 | 0;
                    d = d - 1 | 0;
                }
                d = g & -4 | 0;
                e = d - 64 | 0;
                while ((a | 0) <= (e | 0)) {
                    f[a >> 2] = f[c >> 2];
                    f[a + 4 >> 2] = f[c + 4 >> 2];
                    f[a + 8 >> 2] = f[c + 8 >> 2];
                    f[a + 12 >> 2] = f[c + 12 >> 2];
                    f[a + 16 >> 2] = f[c + 16 >> 2];
                    f[a + 20 >> 2] = f[c + 20 >> 2];
                    f[a + 24 >> 2] = f[c + 24 >> 2];
                    f[a + 28 >> 2] = f[c + 28 >> 2];
                    f[a + 32 >> 2] = f[c + 32 >> 2];
                    f[a + 36 >> 2] = f[c + 36 >> 2];
                    f[a + 40 >> 2] = f[c + 40 >> 2];
                    f[a + 44 >> 2] = f[c + 44 >> 2];
                    f[a + 48 >> 2] = f[c + 48 >> 2];
                    f[a + 52 >> 2] = f[c + 52 >> 2];
                    f[a + 56 >> 2] = f[c + 56 >> 2];
                    f[a + 60 >> 2] = f[c + 60 >> 2];
                    a = a + 64 | 0;
                    c = c + 64 | 0;
                }
                while ((a | 0) < (d | 0)) {
                    f[a >> 2] = f[c >> 2];
                    a = a + 4 | 0;
                    c = c + 4 | 0;
                }
            } else {
                d = g - 4 | 0;
                while ((a | 0) < (d | 0)) {
                    b[a >> 0] = b[c >> 0] | 0;
                    b[a + 1 >> 0] = b[c + 1 >> 0] | 0;
                    b[a + 2 >> 0] = b[c + 2 >> 0] | 0;
                    b[a + 3 >> 0] = b[c + 3 >> 0] | 0;
                    a = a + 4 | 0;
                    c = c + 4 | 0;
                }
            }
            while ((a | 0) < (g | 0)) {
                b[a >> 0] = b[c >> 0] | 0;
                a = a + 1 | 0;
                c = c + 1 | 0;
            }
            return h | 0
        }

        function lb(a) {
            a = a | 0;
            var b = 0, c = 0, d = 0, e = 0;
            e = a + 92 | 0;
            d = a + 88 | 0;
            c = f[d >> 2] | 0;
            b = (f[a + 4 >> 2] | 0) + ((h[c + 68 >> 0] | 0) << 8 | (h[c + 67 >> 0] | 0) << 16 | (h[c + 69 >> 0] | 0)) | 0;
            c = (h[c + 65 >> 0] | 0) << 8 | (h[c + 66 >> 0] | 0);
            if (!c) {
                e = 0;
                return e | 0
            }
            f[e >> 2] = b;
            f[a + 96 >> 2] = b;
            f[a + 104 >> 2] = c;
            f[a + 100 >> 2] = b + c;
            f[a + 108 >> 2] = 0;
            f[a + 112 >> 2] = 0;
            if (!(Pa(e, a + 116 | 0) | 0)) {
                e = 0;
                return e | 0
            }
            b = f[d >> 2] | 0;
            do if (!((h[b + 39 >> 0] | 0) << 8 | (h[b + 40 >> 0] | 0))) {
                if (!((h[b + 55 >> 0] | 0) << 8 | (h[b + 56 >> 0] | 0))) {
                    e = 0;
                    return e | 0
                }
            } else {
                if (!(Pa(e, a + 140 | 0) | 0)) {
                    e = 0;
                    return e | 0
                }
                if (Pa(e, a + 188 | 0) | 0) {
                    b = f[d >> 2] | 0;
                    break
                } else {
                    e = 0;
                    return e | 0
                }
            } while (0);
            if ((h[b + 55 >> 0] | 0) << 8 | (h[b + 56 >> 0] | 0) | 0) {
                if (!(Pa(e, a + 164 | 0) | 0)) {
                    e = 0;
                    return e | 0
                }
                if (!(Pa(e, a + 212 | 0) | 0)) {
                    e = 0;
                    return e | 0
                }
            }
            e = 1;
            return e | 0
        }

        function mb(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0;
            m = u;
            u = u + 48 | 0;
            k = m + 16 | 0;
            g = m;
            e = m + 32 | 0;
            i = a + 28 | 0;
            d = f[i >> 2] | 0;
            f[e >> 2] = d;
            j = a + 20 | 0;
            d = (f[j >> 2] | 0) - d | 0;
            f[e + 4 >> 2] = d;
            f[e + 8 >> 2] = b;
            f[e + 12 >> 2] = c;
            d = d + c | 0;
            h = a + 60 | 0;
            f[g >> 2] = f[h >> 2];
            f[g + 4 >> 2] = e;
            f[g + 8 >> 2] = 2;
            g = Ic(Aa(146, g | 0) | 0) | 0;
            a:do if ((d | 0) != (g | 0)) {
                b = 2;
                while (1) {
                    if ((g | 0) < 0) break;
                    d = d - g | 0;
                    o = f[e + 4 >> 2] | 0;
                    n = g >>> 0 > o >>> 0;
                    e = n ? e + 8 | 0 : e;
                    b = (n << 31 >> 31) + b | 0;
                    o = g - (n ? o : 0) | 0;
                    f[e >> 2] = (f[e >> 2] | 0) + o;
                    n = e + 4 | 0;
                    f[n >> 2] = (f[n >> 2] | 0) - o;
                    f[k >> 2] = f[h >> 2];
                    f[k + 4 >> 2] = e;
                    f[k + 8 >> 2] = b;
                    g = Ic(Aa(146, k | 0) | 0) | 0;
                    if ((d | 0) == (g | 0)) {
                        l = 3;
                        break a
                    }
                }
                f[a + 16 >> 2] = 0;
                f[i >> 2] = 0;
                f[j >> 2] = 0;
                f[a >> 2] = f[a >> 2] | 32;
                if ((b | 0) == 2) c = 0; else c = c - (f[e + 4 >> 2] | 0) | 0;
            } else l = 3; while (0);
            if ((l | 0) == 3) {
                o = f[a + 44 >> 2] | 0;
                f[a + 16 >> 2] = o + (f[a + 48 >> 2] | 0);
                f[i >> 2] = o;
                f[j >> 2] = o;
            }
            u = m;
            return c | 0
        }

        function nb(a, c, d, e, g) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            var h = 0, i = 0, j = 0, k = 0;
            do if (!(Sc(a, f[c + 8 >> 2] | 0) | 0)) {
                h = a + 8 | 0;
                if (!(Sc(a, f[c >> 2] | 0) | 0)) {
                    j = f[h >> 2] | 0;
                    Fa[f[(f[j >> 2] | 0) + 24 >> 2] & 3](j, c, d, e, g);
                    break
                }
                a = c + 32 | 0;
                if ((f[c + 16 >> 2] | 0) != (d | 0) ? (i = c + 20 | 0, (f[i >> 2] | 0) != (d | 0)) : 0) {
                    f[a >> 2] = e;
                    e = c + 44 | 0;
                    if ((f[e >> 2] | 0) == 4) break;
                    a = c + 52 | 0;
                    b[a >> 0] = 0;
                    k = c + 53 | 0;
                    b[k >> 0] = 0;
                    h = f[h >> 2] | 0;
                    Ka[f[(f[h >> 2] | 0) + 20 >> 2] & 3](h, c, d, d, 1, g);
                    if (b[k >> 0] | 0) if (!(b[a >> 0] | 0)) {
                        a = 3;
                        j = 11;
                    } else a = 3; else {
                        a = 4;
                        j = 11;
                    }
                    if ((j | 0) == 11) {
                        f[i >> 2] = d;
                        k = c + 40 | 0;
                        f[k >> 2] = (f[k >> 2] | 0) + 1;
                        if ((f[c + 36 >> 2] | 0) == 1 ? (f[c + 24 >> 2] | 0) == 2 : 0) b[c + 54 >> 0] = 1;
                    }
                    f[e >> 2] = a;
                    break
                }
                if ((e | 0) == 1) f[a >> 2] = 1;
            } else jc(0, c, d, e); while (0);
        }

        function ob(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, p = 0, q = 0, r = 0;
            r = u;
            u = u + 224 | 0;
            m = r + 120 | 0;
            n = r + 80 | 0;
            p = r;
            q = r + 136 | 0;
            e = n;
            g = e + 40 | 0;
            do {
                f[e >> 2] = 0;
                e = e + 4 | 0;
            } while ((e | 0) < (g | 0));
            f[m >> 2] = f[d >> 2];
            if ((Oa(0, c, m, p, n) | 0) < 0) d = -1; else {
                if ((f[a + 76 >> 2] | 0) > -1) ;
                d = f[a >> 2] | 0;
                l = d & 32;
                if ((b[a + 74 >> 0] | 0) < 1) f[a >> 2] = d & -33;
                e = a + 48 | 0;
                if (!(f[e >> 2] | 0)) {
                    g = a + 44 | 0;
                    h = f[g >> 2] | 0;
                    f[g >> 2] = q;
                    i = a + 28 | 0;
                    f[i >> 2] = q;
                    j = a + 20 | 0;
                    f[j >> 2] = q;
                    f[e >> 2] = 80;
                    k = a + 16 | 0;
                    f[k >> 2] = q + 80;
                    d = Oa(a, c, m, p, n) | 0;
                    if (h) {
                        Ea[f[a + 36 >> 2] & 7](a, 0, 0) | 0;
                        d = (f[j >> 2] | 0) == 0 ? -1 : d;
                        f[g >> 2] = h;
                        f[e >> 2] = 0;
                        f[k >> 2] = 0;
                        f[i >> 2] = 0;
                        f[j >> 2] = 0;
                    }
                } else d = Oa(a, c, m, p, n) | 0;
                e = f[a >> 2] | 0;
                f[a >> 2] = e | l;
                d = (e & 32 | 0) == 0 ? d : -1;
            }
            u = r;
            return d | 0
        }

        function pb(a, c, e, g) {
            a = a | 0;
            c = c | 0;
            e = e | 0;
            g = g | 0;
            var h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0;
            p = u;
            u = u + 64 | 0;
            n = p;
            m = f[a >> 2] | 0;
            o = a + (f[m + -8 >> 2] | 0) | 0;
            m = f[m + -4 >> 2] | 0;
            f[n >> 2] = e;
            f[n + 4 >> 2] = a;
            f[n + 8 >> 2] = c;
            f[n + 12 >> 2] = g;
            a = n + 16 | 0;
            c = n + 20 | 0;
            g = n + 24 | 0;
            h = n + 28 | 0;
            i = n + 32 | 0;
            j = n + 40 | 0;
            k = a;
            l = k + 36 | 0;
            do {
                f[k >> 2] = 0;
                k = k + 4 | 0;
            } while ((k | 0) < (l | 0));
            d[a + 36 >> 1] = 0;
            b[a + 38 >> 0] = 0;
            a:do if (Sc(m, e) | 0) {
                f[n + 48 >> 2] = 1;
                Ka[f[(f[m >> 2] | 0) + 20 >> 2] & 3](m, n, o, o, 1, 0);
                a = (f[g >> 2] | 0) == 1 ? o : 0;
            } else {
                Fa[f[(f[m >> 2] | 0) + 24 >> 2] & 3](m, n, o, 1, 0);
                switch (f[n + 36 >> 2] | 0) {
                    case 0: {
                        a = (f[j >> 2] | 0) == 1 & (f[h >> 2] | 0) == 1 & (f[i >> 2] | 0) == 1 ? f[c >> 2] | 0 : 0;
                        break a
                    }
                    case 1:
                        break;
                    default: {
                        a = 0;
                        break a
                    }
                }
                if ((f[g >> 2] | 0) != 1 ? !((f[j >> 2] | 0) == 0 & (f[h >> 2] | 0) == 1 & (f[i >> 2] | 0) == 1) : 0) {
                    a = 0;
                    break
                }
                a = f[a >> 2] | 0;
            } while (0);
            u = p;
            return a | 0
        }

        function qb(a) {
            a = a | 0;
            var b = 0, c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0;
            j = u;
            u = u + 544 | 0;
            h = j + 16 | 0;
            b = j;
            e = j + 32 | 0;
            g = a + 8 | 0;
            c = f[g >> 2] | 0;
            if ((c + -1 | 0) >>> 0 >= 8192) {
                f[b >> 2] = 866;
                f[b + 4 >> 2] = 3006;
                f[b + 8 >> 2] = 1257;
                vc(e, 812, b) | 0;
                Ub(e) | 0;
            }
            f[a >> 2] = c;
            d = a + 20 | 0;
            b = f[d >> 2] | 0;
            if (!b) {
                b = Db(180, 0) | 0;
                if (!b) b = 0; else {
                    i = b + 164 | 0;
                    f[i >> 2] = 0;
                    f[i + 4 >> 2] = 0;
                    f[i + 8 >> 2] = 0;
                    f[i + 12 >> 2] = 0;
                }
                f[d >> 2] = b;
                i = f[a >> 2] | 0;
            } else i = c;
            if (!(f[g >> 2] | 0)) {
                f[h >> 2] = 866;
                f[h + 4 >> 2] = 910;
                f[h + 8 >> 2] = 1497;
                vc(e, 812, h) | 0;
                Ub(e) | 0;
                h = f[a >> 2] | 0;
            } else h = i;
            e = f[a + 4 >> 2] | 0;
            if (h >>> 0 > 16) {
                c = h;
                d = 0;
            } else {
                a = 0;
                a = Qa(b, i, e, a) | 0;
                u = j;
                return a | 0
            }
            while (1) {
                g = d + 1 | 0;
                if (c >>> 0 > 3) {
                    c = c >>> 1;
                    d = g;
                } else break
            }
            a = d + 2 + ((g | 0) != 32 & 1 << g >>> 0 < h >>> 0 & 1) | 0;
            a = (a >>> 0 < 11 ? a : 11) & 255;
            a = Qa(b, i, e, a) | 0;
            u = j;
            return a | 0
        }

        function rb(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0;
            o = (f[a >> 2] | 0) + 1794895138 | 0;
            h = Tc(f[a + 8 >> 2] | 0, o) | 0;
            e = Tc(f[a + 12 >> 2] | 0, o) | 0;
            g = Tc(f[a + 16 >> 2] | 0, o) | 0;
            a:do if ((h >>> 0 < c >>> 2 >>> 0 ? (n = c - (h << 2) | 0, e >>> 0 < n >>> 0 & g >>> 0 < n >>> 0) : 0) ? ((g | e) & 3 | 0) == 0 : 0) {
                n = e >>> 2;
                m = g >>> 2;
                l = 0;
                while (1) {
                    j = h >>> 1;
                    k = l + j | 0;
                    i = k << 1;
                    g = i + n | 0;
                    e = Tc(f[a + (g << 2) >> 2] | 0, o) | 0;
                    g = Tc(f[a + (g + 1 << 2) >> 2] | 0, o) | 0;
                    if (!(g >>> 0 < c >>> 0 & e >>> 0 < (c - g | 0) >>> 0)) {
                        e = 0;
                        break a
                    }
                    if (b[a + (g + e) >> 0] | 0) {
                        e = 0;
                        break a
                    }
                    e = _b(d, a + g | 0) | 0;
                    if (!e) break;
                    e = (e | 0) < 0;
                    if ((h | 0) == 1) {
                        e = 0;
                        break a
                    } else {
                        l = e ? l : k;
                        h = e ? j : h - j | 0;
                    }
                }
                e = i + m | 0;
                g = Tc(f[a + (e << 2) >> 2] | 0, o) | 0;
                e = Tc(f[a + (e + 1 << 2) >> 2] | 0, o) | 0;
                if (e >>> 0 < c >>> 0 & g >>> 0 < (c - e | 0) >>> 0) e = (b[a + (e + g) >> 0] | 0) == 0 ? a + e | 0 : 0; else e = 0;
            } else e = 0; while (0);
            return e | 0
        }

        function sb(a) {
            a = a | 0;
            var b = 0, c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0;
            i = u;
            u = u + 576 | 0;
            g = i + 48 | 0;
            h = i + 32 | 0;
            d = i + 16 | 0;
            c = i;
            e = i + 64 | 0;
            b = f[a + 168 >> 2] | 0;
            do if (b | 0) {
                j = f[b + -4 >> 2] | 0;
                b = b + -8 | 0;
                if (!((j | 0) != 0 ? (j | 0) == (~f[b >> 2] | 0) : 0)) {
                    f[c >> 2] = 866;
                    f[c + 4 >> 2] = 651;
                    f[c + 8 >> 2] = 1579;
                    vc(e, 812, c) | 0;
                    Ub(e) | 0;
                }
                if (!(b & 7)) {
                    Nb(b, 0, 0, 1, 0) | 0;
                    break
                } else {
                    f[d >> 2] = 866;
                    f[d + 4 >> 2] = 2506;
                    f[d + 8 >> 2] = 1232;
                    vc(e, 812, d) | 0;
                    Ub(e) | 0;
                    break
                }
            } while (0);
            b = f[a + 176 >> 2] | 0;
            if (!b) {
                u = i;
                return
            }
            j = f[b + -4 >> 2] | 0;
            b = b + -8 | 0;
            if (!((j | 0) != 0 ? (j | 0) == (~f[b >> 2] | 0) : 0)) {
                f[h >> 2] = 866;
                f[h + 4 >> 2] = 651;
                f[h + 8 >> 2] = 1579;
                vc(e, 812, h) | 0;
                Ub(e) | 0;
            }
            if (!(b & 7)) {
                Nb(b, 0, 0, 1, 0) | 0;
                u = i;
            } else {
                f[g >> 2] = 866;
                f[g + 4 >> 2] = 2506;
                f[g + 8 >> 2] = 1232;
                vc(e, 812, g) | 0;
                Ub(e) | 0;
                u = i;
            }
        }

        function tb(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0;
            if (!((a | 0) != 0 & c >>> 0 > 73 & (d | 0) != 0)) {
                d = 0;
                return d | 0
            }
            if ((f[d >> 2] | 0) != 40) {
                d = 0;
                return d | 0
            }
            if (((h[a >> 0] | 0) << 8 | (h[a + 1 >> 0] | 0) | 0) != 18552) {
                d = 0;
                return d | 0
            }
            if (((h[a + 2 >> 0] | 0) << 8 | (h[a + 3 >> 0] | 0)) >>> 0 < 74) {
                d = 0;
                return d | 0
            }
            if (((h[a + 7 >> 0] | 0) << 16 | (h[a + 6 >> 0] | 0) << 24 | (h[a + 8 >> 0] | 0) << 8 | (h[a + 9 >> 0] | 0)) >>> 0 > c >>> 0) {
                d = 0;
                return d | 0
            }
            f[d + 4 >> 2] = (h[a + 12 >> 0] | 0) << 8 | (h[a + 13 >> 0] | 0);
            f[d + 8 >> 2] = (h[a + 14 >> 0] | 0) << 8 | (h[a + 15 >> 0] | 0);
            f[d + 12 >> 2] = h[a + 16 >> 0];
            f[d + 16 >> 2] = h[a + 17 >> 0];
            c = a + 18 | 0;
            e = d + 32 | 0;
            f[e >> 2] = h[c >> 0];
            f[e + 4 >> 2] = 0;
            c = b[c >> 0] | 0;
            f[d + 20 >> 2] = c << 24 >> 24 == 0 | c << 24 >> 24 == 9 ? 8 : 16;
            f[d + 24 >> 2] = (h[a + 26 >> 0] | 0) << 16 | (h[a + 25 >> 0] | 0) << 24 | (h[a + 27 >> 0] | 0) << 8 | (h[a + 28 >> 0] | 0);
            f[d + 28 >> 2] = (h[a + 30 >> 0] | 0) << 16 | (h[a + 29 >> 0] | 0) << 24 | (h[a + 31 >> 0] | 0) << 8 | (h[a + 32 >> 0] | 0);
            d = 1;
            return d | 0
        }

        function ub(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0;
            l = u;
            u = u + 544 | 0;
            j = l + 16 | 0;
            c = l;
            i = l + 32 | 0;
            if (b >>> 0 >= 33) {
                f[c >> 2] = 866;
                f[c + 4 >> 2] = 3199;
                f[c + 8 >> 2] = 1350;
                vc(i, 812, c) | 0;
                Ub(i) | 0;
            }
            k = a + 20 | 0;
            c = f[k >> 2] | 0;
            if ((c | 0) >= (b | 0)) {
                e = a + 16 | 0;
                g = e;
                e = f[e >> 2] | 0;
                i = c;
                j = 32 - b | 0;
                j = e >>> j;
                e = e << b;
                f[g >> 2] = e;
                b = i - b | 0;
                f[k >> 2] = b;
                u = l;
                return j | 0
            }
            e = a + 4 | 0;
            g = a + 8 | 0;
            d = a + 16 | 0;
            do {
                a = f[e >> 2] | 0;
                if ((a | 0) == (f[g >> 2] | 0)) a = 0; else {
                    f[e >> 2] = a + 1;
                    a = h[a >> 0] | 0;
                }
                c = c + 8 | 0;
                f[k >> 2] = c;
                if ((c | 0) >= 33) {
                    f[j >> 2] = 866;
                    f[j + 4 >> 2] = 3208;
                    f[j + 8 >> 2] = 1366;
                    vc(i, 812, j) | 0;
                    Ub(i) | 0;
                    c = f[k >> 2] | 0;
                }
                a = a << 32 - c | f[d >> 2];
                f[d >> 2] = a;
            } while ((c | 0) < (b | 0));
            j = 32 - b | 0;
            j = a >>> j;
            i = a << b;
            f[d >> 2] = i;
            b = c - b | 0;
            f[k >> 2] = b;
            u = l;
            return j | 0
        }

        function vb(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0, i = 0;
            h = c & 255;
            e = (d | 0) != 0;
            a:do if (e & (a & 3 | 0) != 0) {
                g = c & 255;
                while (1) {
                    if ((b[a >> 0] | 0) == g << 24 >> 24) {
                        i = 6;
                        break a
                    }
                    a = a + 1 | 0;
                    d = d + -1 | 0;
                    e = (d | 0) != 0;
                    if (!(e & (a & 3 | 0) != 0)) {
                        i = 5;
                        break
                    }
                }
            } else i = 5; while (0);
            if ((i | 0) == 5) if (e) i = 6; else d = 0;
            b:do if ((i | 0) == 6) {
                g = c & 255;
                if ((b[a >> 0] | 0) != g << 24 >> 24) {
                    e = X(h, 16843009) | 0;
                    c:do if (d >>> 0 > 3) while (1) {
                        h = f[a >> 2] ^ e;
                        if ((h & -2139062144 ^ -2139062144) & h + -16843009 | 0) break;
                        a = a + 4 | 0;
                        d = d + -4 | 0;
                        if (d >>> 0 <= 3) {
                            i = 11;
                            break c
                        }
                    } else i = 11; while (0);
                    if ((i | 0) == 11) if (!d) {
                        d = 0;
                        break
                    }
                    while (1) {
                        if ((b[a >> 0] | 0) == g << 24 >> 24) break b;
                        a = a + 1 | 0;
                        d = d + -1 | 0;
                        if (!d) {
                            d = 0;
                            break
                        }
                    }
                }
            } while (0);
            return (d | 0 ? a : 0) | 0
        }

        function wb(a, b, c, d, e) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var g = 0, i = 0, j = 0, k = 0, l = 0, m = 0;
            m = u;
            u = u + 528 | 0;
            l = m;
            k = m + 16 | 0;
            i = f[a + 88 >> 2] | 0;
            j = (h[i + 70 + (e << 2) + 1 >> 0] | 0) << 16 | (h[i + 70 + (e << 2) >> 0] | 0) << 24 | (h[i + 70 + (e << 2) + 2 >> 0] | 0) << 8 | (h[i + 70 + (e << 2) + 3 >> 0] | 0);
            g = e + 1 | 0;
            if (g >>> 0 < (h[i + 16 >> 0] | 0) >>> 0) g = (h[i + 70 + (g << 2) + 1 >> 0] | 0) << 16 | (h[i + 70 + (g << 2) >> 0] | 0) << 24 | (h[i + 70 + (g << 2) + 2 >> 0] | 0) << 8 | (h[i + 70 + (g << 2) + 3 >> 0] | 0); else g = f[a + 8 >> 2] | 0;
            if (g >>> 0 > j >>> 0) {
                k = a + 4 | 0;
                k = f[k >> 2] | 0;
                k = k + j | 0;
                l = g - j | 0;
                l = jb(a, k, l, b, c, d, e) | 0;
                u = m;
                return l | 0
            }
            f[l >> 2] = 866;
            f[l + 4 >> 2] = 3694;
            f[l + 8 >> 2] = 1508;
            vc(k, 812, l) | 0;
            Ub(k) | 0;
            k = a + 4 | 0;
            k = f[k >> 2] | 0;
            k = k + j | 0;
            l = g - j | 0;
            l = jb(a, k, l, b, c, d, e) | 0;
            u = m;
            return l | 0
        }

        function xb(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0;
            if (((!((b | 0) == 0 | c >>> 0 < 74) ? ((h[b >> 0] | 0) << 8 | (h[b + 1 >> 0] | 0) | 0) == 18552 : 0) ? ((h[b + 2 >> 0] | 0) << 8 | (h[b + 3 >> 0] | 0)) >>> 0 >= 74 : 0) ? ((h[b + 7 >> 0] | 0) << 16 | (h[b + 6 >> 0] | 0) << 24 | (h[b + 8 >> 0] | 0) << 8 | (h[b + 9 >> 0] | 0)) >>> 0 <= c >>> 0 : 0) {
                d = a + 88 | 0;
                f[d >> 2] = b;
                f[a + 4 >> 2] = b;
                f[a + 8 >> 2] = c;
                if (!(lb(a) | 0)) {
                    e = 0;
                    return e | 0
                }
                b = f[d >> 2] | 0;
                if ((h[b + 39 >> 0] | 0) << 8 | (h[b + 40 >> 0] | 0)) {
                    if (ab(a) | 0 ? Xa(a) | 0 : 0) {
                        b = f[d >> 2] | 0;
                        e = 11;
                    }
                } else e = 11;
                if ((e | 0) == 11) {
                    if (!((h[b + 55 >> 0] | 0) << 8 | (h[b + 56 >> 0] | 0))) {
                        e = 1;
                        return e | 0
                    }
                    if (eb(a) | 0 ? Wa(a) | 0 : 0) {
                        e = 1;
                        return e | 0
                    }
                }
                e = 0;
                return e | 0
            }
            f[a + 88 >> 2] = 0;
            e = 0;
            return e | 0
        }

        function yb(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0;
            l = u;
            u = u + 528 | 0;
            i = l;
            g = l + 16 | 0;
            if (!b) {
                k = 0;
                u = l;
                return k | 0
            }
            if (b >>> 0 <= 16) {
                k = ub(a, b) | 0;
                u = l;
                return k | 0
            }
            j = ub(a, b + -16 | 0) | 0;
            k = a + 20 | 0;
            b = f[k >> 2] | 0;
            if ((b | 0) < 16) {
                d = a + 4 | 0;
                e = a + 8 | 0;
                c = a + 16 | 0;
                do {
                    a = f[d >> 2] | 0;
                    if ((a | 0) == (f[e >> 2] | 0)) a = 0; else {
                        f[d >> 2] = a + 1;
                        a = h[a >> 0] | 0;
                    }
                    b = b + 8 | 0;
                    f[k >> 2] = b;
                    if ((b | 0) >= 33) {
                        f[i >> 2] = 866;
                        f[i + 4 >> 2] = 3208;
                        f[i + 8 >> 2] = 1366;
                        vc(g, 812, i) | 0;
                        Ub(g) | 0;
                        b = f[k >> 2] | 0;
                    }
                    a = a << 32 - b | f[c >> 2];
                    f[c >> 2] = a;
                } while ((b | 0) < 16)
            } else {
                a = a + 16 | 0;
                c = a;
                a = f[a >> 2] | 0;
            }
            f[c >> 2] = a << 16;
            f[k >> 2] = b + -16;
            k = a >>> 16 | j << 16;
            u = l;
            return k | 0
        }

        function zb(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0, i = 0, j = 0;
            e = d + 16 | 0;
            g = f[e >> 2] | 0;
            if (!g) if (!(Zb(d) | 0)) {
                g = f[e >> 2] | 0;
                h = 5;
            } else e = 0; else h = 5;
            a:do if ((h | 0) == 5) {
                j = d + 20 | 0;
                i = f[j >> 2] | 0;
                e = i;
                if ((g - i | 0) >>> 0 < c >>> 0) {
                    e = Ea[f[d + 36 >> 2] & 7](d, a, c) | 0;
                    break
                }
                b:do if ((b[d + 75 >> 0] | 0) > -1) {
                    i = c;
                    while (1) {
                        if (!i) {
                            h = 0;
                            g = a;
                            break b
                        }
                        g = i + -1 | 0;
                        if ((b[a + g >> 0] | 0) == 10) break; else i = g;
                    }
                    e = Ea[f[d + 36 >> 2] & 7](d, a, i) | 0;
                    if (e >>> 0 < i >>> 0) break a;
                    h = i;
                    g = a + i | 0;
                    c = c - i | 0;
                    e = f[j >> 2] | 0;
                } else {
                    h = 0;
                    g = a;
                } while (0);
                kb(e | 0, g | 0, c | 0) | 0;
                f[j >> 2] = (f[j >> 2] | 0) + c;
                e = h + c | 0;
            } while (0);
            return e | 0
        }

        function Ab(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            do if (a) {
                if (c >>> 0 < 128) {
                    b[a >> 0] = c;
                    a = 1;
                    break
                }
                d = (ld() | 0) + 188 | 0;
                if (!(f[f[d >> 2] >> 2] | 0)) if ((c & -128 | 0) == 57216) {
                    b[a >> 0] = c;
                    a = 1;
                    break
                } else {
                    a = jd() | 0;
                    f[a >> 2] = 84;
                    a = -1;
                    break
                }
                if (c >>> 0 < 2048) {
                    b[a >> 0] = c >>> 6 | 192;
                    b[a + 1 >> 0] = c & 63 | 128;
                    a = 2;
                    break
                }
                if (c >>> 0 < 55296 | (c & -8192 | 0) == 57344) {
                    b[a >> 0] = c >>> 12 | 224;
                    b[a + 1 >> 0] = c >>> 6 & 63 | 128;
                    b[a + 2 >> 0] = c & 63 | 128;
                    a = 3;
                    break
                }
                if ((c + -65536 | 0) >>> 0 < 1048576) {
                    b[a >> 0] = c >>> 18 | 240;
                    b[a + 1 >> 0] = c >>> 12 & 63 | 128;
                    b[a + 2 >> 0] = c >>> 6 & 63 | 128;
                    b[a + 3 >> 0] = c & 63 | 128;
                    a = 4;
                    break
                } else {
                    a = jd() | 0;
                    f[a >> 2] = 84;
                    a = -1;
                    break
                }
            } else a = 1; while (0);
            return a | 0
        }

        function Bb() {
            var a = 0, b = 0, c = 0, d = 0, e = 0, g = 0, h = 0, i = 0;
            e = u;
            u = u + 48 | 0;
            h = e + 32 | 0;
            c = e + 24 | 0;
            i = e + 16 | 0;
            g = e;
            e = e + 36 | 0;
            a = rc() | 0;
            if (a | 0 ? (d = f[a >> 2] | 0, d | 0) : 0) {
                a = d + 48 | 0;
                b = f[a >> 2] | 0;
                a = f[a + 4 >> 2] | 0;
                if (!((b & -256 | 0) == 1126902528 & (a | 0) == 1129074247)) {
                    f[c >> 2] = 4168;
                    Ac(4118, c);
                }
                if ((b | 0) == 1126902529 & (a | 0) == 1129074247) a = f[d + 44 >> 2] | 0; else a = d + 80 | 0;
                f[e >> 2] = a;
                d = f[d >> 2] | 0;
                a = f[d + 4 >> 2] | 0;
                if (Ea[f[(f[2] | 0) + 16 >> 2] & 7](8, d, e) | 0) {
                    i = f[e >> 2] | 0;
                    i = Ha[f[(f[i >> 2] | 0) + 8 >> 2] & 1](i) | 0;
                    f[g >> 2] = 4168;
                    f[g + 4 >> 2] = a;
                    f[g + 8 >> 2] = i;
                    Ac(4032, g);
                } else {
                    f[i >> 2] = 4168;
                    f[i + 4 >> 2] = a;
                    Ac(4077, i);
                }
            }
            Ac(4156, h);
        }

        function Cb(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0, g = 0, h = 0;
            h = u;
            u = u + 544 | 0;
            g = h + 16 | 0;
            d = h;
            e = h + 32 | 0;
            c = f[a + 20 >> 2] | 0;
            do if (c | 0) {
                sb(c);
                if (!(c & 7)) {
                    Nb(c, 0, 0, 1, 0) | 0;
                    break
                } else {
                    f[d >> 2] = 866;
                    f[d + 4 >> 2] = 2506;
                    f[d + 8 >> 2] = 1232;
                    vc(e, 812, d) | 0;
                    Ub(e) | 0;
                    break
                }
            } while (0);
            c = a + 4 | 0;
            d = f[c >> 2] | 0;
            if (!d) {
                g = a + 16 | 0;
                b[g >> 0] = 0;
                u = h;
                return
            }
            if (!(d & 7)) Nb(d, 0, 0, 1, 0) | 0; else {
                f[g >> 2] = 866;
                f[g + 4 >> 2] = 2506;
                f[g + 8 >> 2] = 1232;
                vc(e, 812, g) | 0;
                Ub(e) | 0;
            }
            f[c >> 2] = 0;
            f[a + 8 >> 2] = 0;
            f[a + 12 >> 2] = 0;
            g = a + 16 | 0;
            b[g >> 0] = 0;
            u = h;
        }

        function Db(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0, e = 0, g = 0, h = 0, i = 0, j = 0;
            j = u;
            u = u + 560 | 0;
            i = j + 32 | 0;
            h = j + 16 | 0;
            c = j;
            g = j + 48 | 0;
            e = j + 44 | 0;
            d = a + 3 & -4;
            d = d | 0 ? d : 4;
            if (d >>> 0 > 2147418112) {
                f[c >> 2] = 866;
                f[c + 4 >> 2] = 2506;
                f[c + 8 >> 2] = 1103;
                vc(g, 812, c) | 0;
                Ub(g) | 0;
                i = 0;
                u = j;
                return i | 0
            }
            f[e >> 2] = d;
            a = Nb(0, d, e, 1, 0) | 0;
            c = f[e >> 2] | 0;
            if (b | 0) f[b >> 2] = c;
            if (!((a | 0) == 0 | c >>> 0 < d >>> 0)) {
                if (a & 7) {
                    f[i >> 2] = 866;
                    f[i + 4 >> 2] = 2533;
                    f[i + 8 >> 2] = 1156;
                    vc(g, 812, i) | 0;
                    Ub(g) | 0;
                }
            } else {
                f[h >> 2] = 866;
                f[h + 4 >> 2] = 2506;
                f[h + 8 >> 2] = 1129;
                vc(g, 812, h) | 0;
                Ub(g) | 0;
                a = 0;
            }
            i = a;
            u = j;
            return i | 0
        }

        function Eb(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0, g = 0, h = 0, i = 0;
            i = u;
            u = u + 544 | 0;
            h = i + 16 | 0;
            e = i;
            g = i + 32 | 0;
            f[a >> 2] = 0;
            c = a + 4 | 0;
            d = f[c >> 2] | 0;
            if (d | 0) {
                if (!(d & 7)) Nb(d, 0, 0, 1, 0) | 0; else {
                    f[e >> 2] = 866;
                    f[e + 4 >> 2] = 2506;
                    f[e + 8 >> 2] = 1232;
                    vc(g, 812, e) | 0;
                    Ub(g) | 0;
                }
                f[c >> 2] = 0;
                f[a + 8 >> 2] = 0;
                f[a + 12 >> 2] = 0;
            }
            b[a + 16 >> 0] = 0;
            a = a + 20 | 0;
            c = f[a >> 2] | 0;
            if (!c) {
                u = i;
                return
            }
            sb(c);
            if (!(c & 7)) Nb(c, 0, 0, 1, 0) | 0; else {
                f[h >> 2] = 866;
                f[h + 4 >> 2] = 2506;
                f[h + 8 >> 2] = 1232;
                vc(g, 812, h) | 0;
                Ub(g) | 0;
            }
            f[a >> 2] = 0;
            u = i;
        }

        function Fb(a, c, d, e) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0;
            m = u;
            u = u + 128 | 0;
            g = m + 124 | 0;
            l = m;
            h = l;
            i = 604;
            j = h + 124 | 0;
            do {
                f[h >> 2] = f[i >> 2];
                h = h + 4 | 0;
                i = i + 4 | 0;
            } while ((h | 0) < (j | 0));
            if ((c + -1 | 0) >>> 0 > 2147483646) if (!c) {
                a = g;
                c = 1;
                k = 4;
            } else {
                c = jd() | 0;
                f[c >> 2] = 75;
                c = -1;
            } else k = 4;
            if ((k | 0) == 4) {
                k = -2 - a | 0;
                k = c >>> 0 > k >>> 0 ? k : c;
                f[l + 48 >> 2] = k;
                g = l + 20 | 0;
                f[g >> 2] = a;
                f[l + 44 >> 2] = a;
                c = a + k | 0;
                a = l + 16 | 0;
                f[a >> 2] = c;
                f[l + 28 >> 2] = c;
                c = ob(l, d, e) | 0;
                if (k) {
                    l = f[g >> 2] | 0;
                    b[l + (((l | 0) == (f[a >> 2] | 0)) << 31 >> 31) >> 0] = 0;
                }
            }
            u = m;
            return c | 0
        }

        function Gb(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0, i = 0, j = 0, k = 0;
            k = u;
            u = u + 560 | 0;
            j = k + 32 | 0;
            g = k + 16 | 0;
            e = k;
            h = k + 48 | 0;
            i = k + 44 | 0;
            if (a & 7 | 0) {
                f[e >> 2] = 866;
                f[e + 4 >> 2] = 2506;
                f[e + 8 >> 2] = 1210;
                vc(h, 812, e) | 0;
                Ub(h) | 0;
                j = 0;
                u = k;
                return j | 0
            }
            if (b >>> 0 > 2147418112) {
                f[g >> 2] = 866;
                f[g + 4 >> 2] = 2506;
                f[g + 8 >> 2] = 1103;
                vc(h, 812, g) | 0;
                Ub(h) | 0;
                j = 0;
                u = k;
                return j | 0
            }
            f[i >> 2] = b;
            a = Nb(a, b, i, d, 0) | 0;
            if (c | 0) f[c >> 2] = f[i >> 2];
            if (a & 7 | 0) {
                f[j >> 2] = 866;
                f[j + 4 >> 2] = 2558;
                f[j + 8 >> 2] = 1156;
                vc(h, 812, j) | 0;
                Ub(h) | 0;
            }
            j = a;
            u = k;
            return j | 0
        }

        function Hb(a, c, d, e, g) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var h = 0;
            do if (!(Sc(a, f[c + 8 >> 2] | 0) | 0)) {
                if (Sc(a, f[c >> 2] | 0) | 0) {
                    a = c + 32 | 0;
                    if ((f[c + 16 >> 2] | 0) != (d | 0) ? (h = c + 20 | 0, (f[h >> 2] | 0) != (d | 0)) : 0) {
                        f[a >> 2] = e;
                        f[h >> 2] = d;
                        e = c + 40 | 0;
                        f[e >> 2] = (f[e >> 2] | 0) + 1;
                        if ((f[c + 36 >> 2] | 0) == 1 ? (f[c + 24 >> 2] | 0) == 2 : 0) b[c + 54 >> 0] = 1;
                        f[c + 44 >> 2] = 4;
                        break
                    }
                    if ((e | 0) == 1) f[a >> 2] = 1;
                }
            } else jc(0, c, d, e); while (0);
        }

        function Ib(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0, h = 0, i = 0;
            h = a + d | 0;
            c = c & 255;
            if ((d | 0) >= 67) {
                while (a & 3) {
                    b[a >> 0] = c;
                    a = a + 1 | 0;
                }
                e = h & -4 | 0;
                g = e - 64 | 0;
                i = c | c << 8 | c << 16 | c << 24;
                while ((a | 0) <= (g | 0)) {
                    f[a >> 2] = i;
                    f[a + 4 >> 2] = i;
                    f[a + 8 >> 2] = i;
                    f[a + 12 >> 2] = i;
                    f[a + 16 >> 2] = i;
                    f[a + 20 >> 2] = i;
                    f[a + 24 >> 2] = i;
                    f[a + 28 >> 2] = i;
                    f[a + 32 >> 2] = i;
                    f[a + 36 >> 2] = i;
                    f[a + 40 >> 2] = i;
                    f[a + 44 >> 2] = i;
                    f[a + 48 >> 2] = i;
                    f[a + 52 >> 2] = i;
                    f[a + 56 >> 2] = i;
                    f[a + 60 >> 2] = i;
                    a = a + 64 | 0;
                }
                while ((a | 0) < (e | 0)) {
                    f[a >> 2] = i;
                    a = a + 4 | 0;
                }
            }
            while ((a | 0) < (h | 0)) {
                b[a >> 0] = c;
                a = a + 1 | 0;
            }
            return h - d | 0
        }

        function Jb(a, c, d, e, g) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            var h = 0, i = 0, j = 0, k = 0;
            b[c + 53 >> 0] = 1;
            do if ((f[c + 4 >> 2] | 0) == (e | 0)) {
                b[c + 52 >> 0] = 1;
                e = c + 16 | 0;
                h = f[e >> 2] | 0;
                j = c + 54 | 0;
                k = c + 48 | 0;
                i = c + 24 | 0;
                a = c + 36 | 0;
                if (!h) {
                    f[e >> 2] = d;
                    f[i >> 2] = g;
                    f[a >> 2] = 1;
                    if (!((f[k >> 2] | 0) == 1 & (g | 0) == 1)) break;
                    b[j >> 0] = 1;
                    break
                }
                if ((h | 0) != (d | 0)) {
                    f[a >> 2] = (f[a >> 2] | 0) + 1;
                    b[j >> 0] = 1;
                    break
                }
                a = f[i >> 2] | 0;
                if ((a | 0) == 2) {
                    f[i >> 2] = g;
                    a = g;
                }
                if ((f[k >> 2] | 0) == 1 & (a | 0) == 1) b[j >> 0] = 1;
            } while (0);
        }

        function Kb(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, h = 0;
            h = u;
            u = u + 64 | 0;
            e = h;
            if (!(Sc(a, b) | 0)) if ((b | 0) != 0 ? (g = pb(b, 32, 16, 0) | 0, (g | 0) != 0) : 0) {
                b = e + 4 | 0;
                d = b + 52 | 0;
                do {
                    f[b >> 2] = 0;
                    b = b + 4 | 0;
                } while ((b | 0) < (d | 0));
                f[e >> 2] = g;
                f[e + 8 >> 2] = a;
                f[e + 12 >> 2] = -1;
                f[e + 48 >> 2] = 1;
                La[f[(f[g >> 2] | 0) + 28 >> 2] & 3](g, e, f[c >> 2] | 0, 1);
                if ((f[e + 24 >> 2] | 0) == 1) {
                    f[c >> 2] = f[e + 16 >> 2];
                    b = 1;
                } else b = 0;
            } else b = 0; else b = 1;
            u = h;
            return b | 0
        }

        function Lb(a, c) {
            a = a | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, i = 0, j = 0, k = 0, l = 0;
            l = u;
            u = u + 16 | 0;
            j = l;
            k = c & 255;
            b[j >> 0] = k;
            e = a + 16 | 0;
            g = f[e >> 2] | 0;
            if (!g) if (!(Zb(a) | 0)) {
                g = f[e >> 2] | 0;
                i = 4;
            } else d = -1; else i = 4;
            do if ((i | 0) == 4) {
                i = a + 20 | 0;
                e = f[i >> 2] | 0;
                if (e >>> 0 < g >>> 0 ? (d = c & 255, (d | 0) != (b[a + 75 >> 0] | 0)) : 0) {
                    f[i >> 2] = e + 1;
                    b[e >> 0] = k;
                    break
                }
                if ((Ea[f[a + 36 >> 2] & 7](a, j, 1) | 0) == 1) d = h[j >> 0] | 0; else d = -1;
            } while (0);
            u = l;
            return d | 0
        }

        function Mb(a, c) {
            a = a | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0, h = 0, i = 0, j = 0, k = 0;
            j = a & 255;
            d = a & 255;
            if ((f[c + 76 >> 2] | 0) >= 0 ? (td() | 0) != 0 : 0) {
                if ((d | 0) != (b[c + 75 >> 0] | 0) ? (h = c + 20 | 0, i = f[h >> 2] | 0, i >>> 0 < (f[c + 16 >> 2] | 0) >>> 0) : 0) {
                    f[h >> 2] = i + 1;
                    b[i >> 0] = j;
                } else d = Lb(c, a) | 0;
            } else k = 3;
            do if ((k | 0) == 3) {
                if ((d | 0) != (b[c + 75 >> 0] | 0) ? (e = c + 20 | 0, g = f[e >> 2] | 0, g >>> 0 < (f[c + 16 >> 2] | 0) >>> 0) : 0) {
                    f[e >> 2] = g + 1;
                    b[g >> 0] = j;
                    break
                }
                d = Lb(c, a) | 0;
            } while (0);
            return d | 0
        }

        function Nb(a, b, c, d, e) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            do if (!a) {
                b = Ma(b) | 0;
                if (c) {
                    if (!b) a = 0; else a = uc(b) | 0;
                    f[c >> 2] = a;
                }
            } else {
                if (!b) {
                    Ta(a);
                    if (!c) {
                        b = 0;
                        break
                    }
                    f[c >> 2] = 0;
                    b = 0;
                    break
                }
                if (d) {
                    b = Sb(a, b) | 0;
                    a = (b | 0) == 0 ? a : b;
                } else b = 0;
                if (c) {
                    e = uc(a) | 0;
                    f[c >> 2] = e;
                }
            } while (0);
            return b | 0
        }

        function Ob(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0;
            e = a;
            a:do if (!(e & 3)) d = 4; else {
                c = e;
                while (1) {
                    if (!(b[a >> 0] | 0)) {
                        a = c;
                        break a
                    }
                    a = a + 1 | 0;
                    c = a;
                    if (!(c & 3)) {
                        d = 4;
                        break
                    }
                }
            } while (0);
            if ((d | 0) == 4) {
                while (1) {
                    c = f[a >> 2] | 0;
                    if (!((c & -2139062144 ^ -2139062144) & c + -16843009)) a = a + 4 | 0; else break
                }
                if ((c & 255) << 24 >> 24) do a = a + 1 | 0; while ((b[a >> 0] | 0) != 0)
            }
            return a - e | 0
        }

        function Pb(a, b) {
            a = +a;
            b = b | 0;
            var c = 0, d = 0, e = 0;
            p[s >> 3] = a;
            c = f[s >> 2] | 0;
            d = f[s + 4 >> 2] | 0;
            e = zc(c | 0, d | 0, 52) | 0;
            switch (e & 2047) {
                case 0: {
                    if (a != 0.0) {
                        a = +Pb(a * 18446744073709551616.0, b);
                        c = (f[b >> 2] | 0) + -64 | 0;
                    } else c = 0;
                    f[b >> 2] = c;
                    break
                }
                case 2047:
                    break;
                default: {
                    f[b >> 2] = (e & 2047) + -1022;
                    f[s >> 2] = c;
                    f[s + 4 >> 2] = d & -2146435073 | 1071644672;
                    a = +p[s >> 3];
                }
            }
            return +a
        }

        function Qb(a, c) {
            a = a | 0;
            c = c | 0;
            var d = 0, e = 0;
            e = 0;
            while (1) {
                if ((h[2140 + e >> 0] | 0) == (a | 0)) {
                    a = 2;
                    break
                }
                d = e + 1 | 0;
                if ((d | 0) == 87) {
                    d = 2228;
                    e = 87;
                    a = 5;
                    break
                } else e = d;
            }
            if ((a | 0) == 2) if (!e) d = 2228; else {
                d = 2228;
                a = 5;
            }
            if ((a | 0) == 5) while (1) {
                do {
                    a = d;
                    d = d + 1 | 0;
                } while ((b[a >> 0] | 0) != 0);
                e = e + -1 | 0;
                if (!e) break; else a = 5;
            }
            return ed(d, f[c + 20 >> 2] | 0) | 0
        }

        function Rb(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0;
            if (c >>> 0 > 0 | (c | 0) == 0 & a >>> 0 > 4294967295) {
                while (1) {
                    e = mc(a | 0, c | 0, 10, 0) | 0;
                    d = d + -1 | 0;
                    b[d >> 0] = e & 255 | 48;
                    e = a;
                    a = Uc(a | 0, c | 0, 10, 0) | 0;
                    if (!(c >>> 0 > 9 | (c | 0) == 9 & e >>> 0 > 4294967295)) break; else c = I;
                }
                c = a;
            } else c = a;
            if (c) while (1) {
                d = d + -1 | 0;
                b[d >> 0] = (c >>> 0) % 10 | 0 | 48;
                if (c >>> 0 < 10) break; else c = (c >>> 0) / 10 | 0;
            }
            return d | 0
        }

        function Sb(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0;
            if (!a) {
                b = Ma(b) | 0;
                return b | 0
            }
            if (b >>> 0 > 4294967231) {
                b = jd() | 0;
                f[b >> 2] = 12;
                b = 0;
                return b | 0
            }
            c = _a(a + -8 | 0, b >>> 0 < 11 ? 16 : b + 11 & -8) | 0;
            if (c | 0) {
                b = c + 8 | 0;
                return b | 0
            }
            c = Ma(b) | 0;
            if (!c) {
                b = 0;
                return b | 0
            }
            d = f[a + -4 >> 2] | 0;
            d = (d & -8) - ((d & 3 | 0) == 0 ? 8 : 4) | 0;
            kb(c | 0, a | 0, (d >>> 0 < b >>> 0 ? d : b) | 0) | 0;
            Ta(a);
            b = c;
            return b | 0
        }

        function Tb(a, c, d, e) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var g = 0, h = 0, i = 0;
            a = c + 16 | 0;
            g = f[a >> 2] | 0;
            h = c + 36 | 0;
            i = c + 24 | 0;
            do if (g) {
                if ((g | 0) != (d | 0)) {
                    f[h >> 2] = (f[h >> 2] | 0) + 1;
                    f[i >> 2] = 2;
                    b[c + 54 >> 0] = 1;
                    break
                }
                if ((f[i >> 2] | 0) == 2) f[i >> 2] = e;
            } else {
                f[a >> 2] = d;
                f[i >> 2] = e;
                f[h >> 2] = 1;
            } while (0);
        }

        function Ub(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0;
            e = f[119] | 0;
            if ((f[e + 76 >> 2] | 0) > -1) ;
            do if ((Mc(a, e) | 0) < 0) a = 1; else {
                if ((b[e + 75 >> 0] | 0) != 10 ? (c = e + 20 | 0, d = f[c >> 2] | 0, d >>> 0 < (f[e + 16 >> 2] | 0) >>> 0) : 0) {
                    f[c >> 2] = d + 1;
                    b[d >> 0] = 10;
                    a = 0;
                    break
                }
                a = (Lb(e, 10) | 0) < 0;
            } while (0);
            return a << 31 >> 31 | 0
        }

        function Vb(a, b, c, d, e, g) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            g = g | 0;
            if (Sc(a, f[b + 8 >> 2] | 0) | 0) Jb(0, b, c, d, e); else {
                a = f[a + 8 >> 2] | 0;
                Ka[f[(f[a >> 2] | 0) + 20 >> 2] & 3](a, b, c, d, e, g);
            }
        }

        function Wb(a, b, c, d, e) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            var f = 0, g = 0;
            g = u;
            u = u + 256 | 0;
            f = g;
            if ((c | 0) > (d | 0) & (e & 73728 | 0) == 0) {
                e = c - d | 0;
                Ib(f | 0, b | 0, (e >>> 0 < 256 ? e : 256) | 0) | 0;
                if (e >>> 0 > 255) {
                    b = c - d | 0;
                    do {
                        Nc(a, f, 256);
                        e = e + -256 | 0;
                    } while (e >>> 0 > 255);
                    e = b & 255;
                }
                Nc(a, f, e);
            }
            u = g;
        }

        function Xb(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            if (Sc(a, f[b + 8 >> 2] | 0) | 0) Tb(0, b, c, d); else {
                a = f[a + 8 >> 2] | 0;
                La[f[(f[a >> 2] | 0) + 28 >> 2] & 3](a, b, c, d);
            }
        }

        function Yb(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0, g = 0;
            e = u;
            u = u + 32 | 0;
            g = e;
            d = e + 20 | 0;
            f[g >> 2] = f[a + 60 >> 2];
            f[g + 4 >> 2] = 0;
            f[g + 8 >> 2] = b;
            f[g + 12 >> 2] = d;
            f[g + 16 >> 2] = c;
            if ((Ic(xa(140, g | 0) | 0) | 0) < 0) {
                f[d >> 2] = -1;
                a = -1;
            } else a = f[d >> 2] | 0;
            u = e;
            return a | 0
        }

        function Zb(a) {
            a = a | 0;
            var c = 0, d = 0;
            c = a + 74 | 0;
            d = b[c >> 0] | 0;
            b[c >> 0] = d + 255 | d;
            c = f[a >> 2] | 0;
            if (!(c & 8)) {
                f[a + 8 >> 2] = 0;
                f[a + 4 >> 2] = 0;
                d = f[a + 44 >> 2] | 0;
                f[a + 28 >> 2] = d;
                f[a + 20 >> 2] = d;
                f[a + 16 >> 2] = d + (f[a + 48 >> 2] | 0);
                a = 0;
            } else {
                f[a >> 2] = c | 32;
                a = -1;
            }
            return a | 0
        }

        function _b(a, c) {
            a = a | 0;
            c = c | 0;
            var d = 0, e = 0;
            d = b[a >> 0] | 0;
            e = b[c >> 0] | 0;
            if (d << 24 >> 24 == 0 ? 1 : d << 24 >> 24 != e << 24 >> 24) a = e; else {
                do {
                    a = a + 1 | 0;
                    c = c + 1 | 0;
                    d = b[a >> 0] | 0;
                    e = b[c >> 0] | 0;
                } while (!(d << 24 >> 24 == 0 ? 1 : d << 24 >> 24 != e << 24 >> 24));
                a = e;
            }
            return (d & 255) - (a & 255) | 0
        }

        function $b(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0;
            g = u;
            u = u + 32 | 0;
            e = g;
            f[a + 36 >> 2] = 1;
            if ((f[a >> 2] & 64 | 0) == 0 ? (f[e >> 2] = f[a + 60 >> 2], f[e + 4 >> 2] = 21523, f[e + 8 >> 2] = g + 16, na(54, e | 0) | 0) : 0) b[a + 75 >> 0] = -1;
            e = mb(a, c, d) | 0;
            u = g;
            return e | 0
        }

        function ac(a) {
            a = a | 0;
            var b = 0, c = 0;
            c = a + 15 & -16 | 0;
            b = f[r >> 2] | 0;
            a = b + c | 0;
            if ((c | 0) > 0 & (a | 0) < (b | 0) | (a | 0) < 0) {
                da() | 0;
                ra(12);
                return -1
            }
            f[r >> 2] = a;
            if ((a | 0) > (ca() | 0) ? (ba() | 0) == 0 : 0) {
                f[r >> 2] = b;
                ra(12);
                return -1
            }
            return b | 0
        }

        function bc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            var e = 0;
            e = X(c, b) | 0;
            c = (b | 0) == 0 ? 0 : c;
            if ((f[d + 76 >> 2] | 0) > -1) {
                a = zb(a, e, d) | 0;
            } else a = zb(a, e, d) | 0;
            if ((a | 0) != (e | 0)) c = (a >>> 0) / (b >>> 0) | 0;
            return c | 0
        }

        function cc(a, b, c, d, e, g) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            if (Sc(a, f[b + 8 >> 2] | 0) | 0) Jb(0, b, c, d, e);
        }

        function dc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            if (Sc(a, f[b + 8 >> 2] | 0) | 0) Tb(0, b, c, d);
        }

        function ec(a) {
            a = a | 0;
            var c = 0, d = 0, e = 0;
            d = f[a >> 2] | 0;
            e = (b[d >> 0] | 0) + -48 | 0;
            if (e >>> 0 < 10) {
                c = 0;
                do {
                    c = e + (c * 10 | 0) | 0;
                    d = d + 1 | 0;
                    f[a >> 2] = d;
                    e = (b[d >> 0] | 0) + -48 | 0;
                } while (e >>> 0 < 10)
            } else c = 0;
            return c | 0
        }

        function gc(a, c, d, e) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            if (!((a | 0) == 0 & (c | 0) == 0)) do {
                d = d + -1 | 0;
                b[d >> 0] = h[2122 + (a & 15) >> 0] | 0 | e;
                a = zc(a | 0, c | 0, 4) | 0;
                c = I;
            } while (!((a | 0) == 0 & (c | 0) == 0));
            return d | 0
        }

        function hc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0;
            e = u;
            u = u + 16 | 0;
            d = e;
            f[d >> 2] = f[c >> 2];
            a = Ea[f[(f[a >> 2] | 0) + 16 >> 2] & 7](a, b, d) | 0;
            if (a) f[c >> 2] = f[d >> 2];
            u = e;
            return a & 1 | 0
        }

        function ic(a) {
            a = a | 0;
            var c = 0;
            c = b[w + (a & 255) >> 0] | 0;
            if ((c | 0) < 8) return c | 0;
            c = b[w + (a >> 8 & 255) >> 0] | 0;
            if ((c | 0) < 8) return c + 8 | 0;
            c = b[w + (a >> 16 & 255) >> 0] | 0;
            if ((c | 0) < 8) return c + 16 | 0;
            return (b[w + (a >>> 24) >> 0] | 0) + 24 | 0
        }

        function jc(a, b, c, d) {
            b = b | 0;
            c = c | 0;
            d = d | 0;
            var e = 0;
            if ((f[b + 4 >> 2] | 0) == (c | 0) ? (e = b + 28 | 0, (f[e >> 2] | 0) != 1) : 0) f[e >> 2] = d;
        }

        function kc(a, c, d) {
            a = a | 0;
            c = c | 0;
            d = d | 0;
            if (!((a | 0) == 0 & (c | 0) == 0)) do {
                d = d + -1 | 0;
                b[d >> 0] = a & 7 | 48;
                a = zc(a | 0, c | 0, 3) | 0;
                c = I;
            } while (!((a | 0) == 0 & (c | 0) == 0));
            return d | 0
        }

        function lc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0;
            d = a + 20 | 0;
            e = f[d >> 2] | 0;
            a = (f[a + 16 >> 2] | 0) - e | 0;
            a = a >>> 0 > c >>> 0 ? c : a;
            kb(e | 0, b | 0, a | 0) | 0;
            f[d >> 2] = (f[d >> 2] | 0) + a;
            return c | 0
        }

        function mc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            var e = 0, g = 0;
            g = u;
            u = u + 16 | 0;
            e = g | 0;
            Za(a, b, c, d, e) | 0;
            u = g;
            return (I = f[e + 4 >> 2] | 0, f[e >> 2] | 0) | 0
        }

        function nc(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0;
            d = u;
            u = u + 48 | 0;
            c = d;
            f[c >> 2] = 40;
            tb(a, b, c) | 0;
            u = d;
            return f[c + 32 >> 2] | 0
        }

        function oc(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0;
            d = u;
            u = u + 48 | 0;
            c = d;
            f[c >> 2] = 40;
            tb(a, b, c) | 0;
            u = d;
            return f[c + 12 >> 2] | 0
        }

        function pc(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0;
            d = u;
            u = u + 48 | 0;
            c = d;
            f[c >> 2] = 40;
            tb(a, b, c) | 0;
            u = d;
            return f[c + 8 >> 2] | 0
        }

        function qc(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0, d = 0;
            d = u;
            u = u + 48 | 0;
            c = d;
            f[c >> 2] = 40;
            tb(a, b, c) | 0;
            u = d;
            return f[c + 4 >> 2] | 0
        }

        function rc() {
            var a = 0, b = 0;
            a = u;
            u = u + 16 | 0;
            if (!(ua(5136, 2) | 0)) {
                b = ma(f[1285] | 0) | 0;
                u = a;
                return b | 0
            } else Ac(4307, a);
            return 0
        }

        function sc(a) {
            a = a | 0;
            var b = 0, c = 0;
            b = u;
            u = u + 16 | 0;
            c = b;
            a = pd(f[a + 60 >> 2] | 0) | 0;
            f[c >> 2] = a;
            a = Ic(qa(6, c | 0) | 0) | 0;
            u = b;
            return a | 0
        }

        function tc(a) {
            a = a | 0;
            var b = 0;
            b = u;
            u = u + 16 | 0;
            Ta(a);
            if (!(oa(f[1285] | 0, 0) | 0)) {
                u = b;
            } else Ac(4406, b);
        }

        function uc(a) {
            a = a | 0;
            var b = 0;
            if (!a) return 0; else {
                b = f[a + -4 >> 2] | 0;
                a = b & 3;
                return ((a | 0) == 1 ? 0 : (b & -8) - ((a | 0) == 0 ? 8 : 4) | 0) | 0
            }
        }

        function vc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            var d = 0, e = 0;
            d = u;
            u = u + 16 | 0;
            e = d;
            f[e >> 2] = c;
            c = Yc(a, b, e) | 0;
            u = d;
            return c | 0
        }

        function wc(a, b, c, d, e, f, g) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            f = f | 0;
            g = g | 0;
            Ka[a & 3](b | 0, c | 0, d | 0, e | 0, f | 0, g | 0);
        }

        function xc() {
            var a = 0;
            a = u;
            u = u + 16 | 0;
            if (!(wa(5140, 6) | 0)) {
                u = a;
            } else Ac(4356, a);
        }

        function yc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            if ((c | 0) < 32) {
                I = b << c | (a & (1 << c) - 1 << 32 - c) >>> 32 - c;
                return a << c
            }
            I = a << c - 32;
            return 0
        }

        function zc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            if ((c | 0) < 32) {
                I = b >>> c;
                return a >>> c | (b & (1 << c) - 1) << 32 - c
            }
            I = 0;
            return b >>> c - 32 | 0
        }

        function Ac(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0;
            c = u;
            u = u + 16 | 0;
            f[c >> 2] = b;
            b = f[26] | 0;
            ob(b, a, c) | 0;
            Mb(10, b) | 0;
            sa();
        }

        function Bc() {
        }

        function Cc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            d = b - d - (c >>> 0 > a >>> 0 | 0) >>> 0;
            return (I = d, a - c >>> 0 | 0) | 0
        }

        function Dc(a, b) {
            a = a | 0;
            b = b | 0;
            if (!b) b = 0; else b = rb(f[b >> 2] | 0, f[b + 4 >> 2] | 0, a) | 0;
            return (b | 0 ? b : a) | 0
        }

        function Ec(a, b, c, d, e, f) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            f = f | 0;
            Fa[a & 3](b | 0, c | 0, d | 0, e | 0, f | 0);
        }

        function Fc(a) {
            a = +a;
            var b = 0;
            p[s >> 3] = a;
            b = f[s >> 2] | 0;
            I = f[s + 4 >> 2] | 0;
            return b | 0
        }

        function Gc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            c = a + c >>> 0;
            return (I = b + d + (c >>> 0 < a >>> 0 | 0) >>> 0, c | 0) | 0
        }

        function Hc(a, b, c, d, e) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            e = e | 0;
            La[a & 3](b | 0, c | 0, d | 0, e | 0);
        }

        function Ic(a) {
            a = a | 0;
            var b = 0;
            if (a >>> 0 > 4294963200) {
                b = jd() | 0;
                f[b >> 2] = 0 - a;
                a = -1;
            }
            return a | 0
        }

        function Kc(a) {
            a = a | 0;
            if (!a) a = 0; else a = (pb(a, 32, 88, 0) | 0) != 0;
            return a & 1 | 0
        }

        function Lc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            return Ea[a & 7](b | 0, c | 0, d | 0) | 0
        }

        function Mc(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0;
            c = Ob(a) | 0;
            return ((bc(a, 1, c, b) | 0) != (c | 0)) << 31 >> 31 | 0
        }

        function Nc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            if (!(f[a >> 2] & 32)) zb(b, c, a) | 0;
        }

        function Oc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            Ia[a & 0](b | 0, c | 0, d | 0);
        }

        function Pc(a) {
            a = a | 0;
            var b = 0;
            b = u;
            u = u + a | 0;
            u = u + 15 & -16;
            return b | 0
        }

        function Qc(a) {
            a = a | 0;
            var b = 0;
            b = (ld() | 0) + 188 | 0;
            return Qb(a, f[b >> 2] | 0) | 0
        }

        function Rc(a, b) {
            a = a | 0;
            b = b | 0;
            if (!a) a = 0; else a = Ab(a, b, 0) | 0;
            return a | 0
        }

        function Sc(a, b, c) {
            a = a | 0;
            b = b | 0;
            return (a | 0) == (b | 0) | 0
        }

        function Tc(a, b) {
            a = a | 0;
            b = b | 0;
            var c = 0;
            c = Vc(a | 0) | 0;
            return ((b | 0) == 0 ? a : c) | 0
        }

        function Uc(a, b, c, d) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            d = d | 0;
            return Za(a, b, c, d, 0) | 0
        }

        function Vc(a) {
            a = a | 0;
            return (a & 255) << 24 | (a >> 8 & 255) << 16 | (a >> 16 & 255) << 8 | a >>> 24 | 0
        }

        function Wc(a, b, c, d, e, f) {
            $(6);
        }

        function Xc(a, b) {
        }

        function Yc(a, b, c) {
            a = a | 0;
            b = b | 0;
            c = c | 0;
            return Fb(a, 2147483647, b, c) | 0
        }

        function Zc(a, b, c, d, e) {
            $(1);
        }

        function _c(a) {
            a = a | 0;
            nd(a);
        }

        function bd(a, b) {
            a = a | 0;
            b = b | 0;
            return Ha[a & 1](b | 0) | 0
        }

        function cd(a, b) {
            a = a | 0;
            b = b | 0;
            u = a;
            v = b;
        }

        function dd(a, b, c, d) {
            $(7);
        }

        function ed(a, b) {
            a = a | 0;
            b = b | 0;
            return Dc(a, b) | 0
        }

        function fd(a, b) {
            a = a | 0;
            b = b | 0;
            Ga[a & 7](b | 0);
        }

        function gd(a, b, c) {
            $(0);
            return 0
        }

        function hd(a, b) {
            a = +a;
            b = b | 0;
            return +(+Pb(a, b))
        }

        function id(a, b, c) {
            $(4);
        }

        function jd() {
            return (ld() | 0) + 64 | 0
        }

        function kd(a) {
            a = a | 0;
            Ja[a & 3]();
        }

        function ld() {
            return xd() | 0
        }

        function md(a) {
            a = a | 0;
            u = a;
        }

        function nd(a) {
            a = a | 0;
            Ta(a);
        }

        function od(a) {
            a = a | 0;
            I = a;
        }

        function pd(a) {
            a = a | 0;
            return a | 0
        }

        function qd() {
            return 5072
        }

        function rd(a) {
            $(3);
            return 0
        }

        function sd(a) {
        }

        function td(a) {
            return 0
        }

        function ud() {
            return I | 0
        }

        function vd() {
            return u | 0
        }

        function wd(a) {
            $(2);
        }

        function xd() {
            return 232
        }

        function yd() {
            $(5);
        }

        // EMSCRIPTEN_END_FUNCS
        var Ea = [gd, mb, Yb, $b, lc, Kb, gd, gd];
        var Fa = [Zc, Hb, nb, Zc];
        var Ga = [wd, sd, _c, sd, sd, _c, tc, wd];
        var Ha = [rd, sc];
        var Ia = [id];
        var Ja = [yd, Bb, xc, yd];
        var Ka = [Wc, cc, Vb, Wc];
        var La = [dd, dc, Xb, dd];
        return {
            stackSave: vd,
            _i64Subtract: Cc,
            _crn_get_bytes_per_block: ib,
            setThrew: Xc,
            dynCall_viii: Oc,
            _bitshift64Lshr: zc,
            _bitshift64Shl: yc,
            dynCall_viiii: Hc,
            setTempRet0: od,
            _crn_decompress: $a,
            _memset: Ib,
            _sbrk: ac,
            _memcpy: kb,
            stackAlloc: Pc,
            _crn_get_height: pc,
            dynCall_vi: fd,
            getTempRet0: ud,
            _crn_get_levels: oc,
            _crn_get_uncompressed_size: hb,
            _i64Add: Gc,
            dynCall_iiii: Lc,
            _emscripten_get_global_libc: qd,
            dynCall_ii: bd,
            ___udivdi3: Uc,
            _llvm_bswap_i32: Vc,
            dynCall_viiiii: Ec,
            ___cxa_can_catch: hc,
            _free: Ta,
            runPostSets: Bc,
            dynCall_viiiiii: wc,
            establishStackSpace: cd,
            ___uremdi3: mc,
            ___cxa_is_pointer_type: Kc,
            stackRestore: md,
            _malloc: Ma,
            _emscripten_replace_memory: Da,
            dynCall_v: kd,
            _crn_get_width: qc,
            _crn_get_dxt_format: nc
        }
    })


        // EMSCRIPTEN_END_ASM
        (Module.asmGlobalArg, Module.asmLibraryArg, buffer);
    var stackSave = Module["stackSave"] = asm["stackSave"];
    var getTempRet0 = Module["getTempRet0"] = asm["getTempRet0"];
    var _memset = Module["_memset"] = asm["_memset"];
    var setThrew = Module["setThrew"] = asm["setThrew"];
    var _bitshift64Lshr = Module["_bitshift64Lshr"] = asm["_bitshift64Lshr"];
    var _bitshift64Shl = Module["_bitshift64Shl"] = asm["_bitshift64Shl"];
    var setTempRet0 = Module["setTempRet0"] = asm["setTempRet0"];
    var _crn_decompress = Module["_crn_decompress"] = asm["_crn_decompress"];
    var _crn_get_bytes_per_block = Module["_crn_get_bytes_per_block"] = asm["_crn_get_bytes_per_block"];
    var _sbrk = Module["_sbrk"] = asm["_sbrk"];
    var _memcpy = Module["_memcpy"] = asm["_memcpy"];
    var stackAlloc = Module["stackAlloc"] = asm["stackAlloc"];
    var _crn_get_height = Module["_crn_get_height"] = asm["_crn_get_height"];
    var _i64Subtract = Module["_i64Subtract"] = asm["_i64Subtract"];
    var _crn_get_levels = Module["_crn_get_levels"] = asm["_crn_get_levels"];
    var _crn_get_uncompressed_size = Module["_crn_get_uncompressed_size"] = asm["_crn_get_uncompressed_size"];
    var _i64Add = Module["_i64Add"] = asm["_i64Add"];
    var _emscripten_get_global_libc = Module["_emscripten_get_global_libc"] = asm["_emscripten_get_global_libc"];
    var ___udivdi3 = Module["___udivdi3"] = asm["___udivdi3"];
    var _llvm_bswap_i32 = Module["_llvm_bswap_i32"] = asm["_llvm_bswap_i32"];
    var ___cxa_can_catch = Module["___cxa_can_catch"] = asm["___cxa_can_catch"];
    var _free = Module["_free"] = asm["_free"];
    var runPostSets = Module["runPostSets"] = asm["runPostSets"];
    var establishStackSpace = Module["establishStackSpace"] = asm["establishStackSpace"];
    var ___uremdi3 = Module["___uremdi3"] = asm["___uremdi3"];
    var ___cxa_is_pointer_type = Module["___cxa_is_pointer_type"] = asm["___cxa_is_pointer_type"];
    var stackRestore = Module["stackRestore"] = asm["stackRestore"];
    var _malloc = Module["_malloc"] = asm["_malloc"];
    var _emscripten_replace_memory = Module["_emscripten_replace_memory"] = asm["_emscripten_replace_memory"];
    var _crn_get_width = Module["_crn_get_width"] = asm["_crn_get_width"];
    var _crn_get_dxt_format = Module["_crn_get_dxt_format"] = asm["_crn_get_dxt_format"];
    var dynCall_iiii = Module["dynCall_iiii"] = asm["dynCall_iiii"];
    var dynCall_viiiii = Module["dynCall_viiiii"] = asm["dynCall_viiiii"];
    var dynCall_vi = Module["dynCall_vi"] = asm["dynCall_vi"];
    var dynCall_ii = Module["dynCall_ii"] = asm["dynCall_ii"];
    var dynCall_viii = Module["dynCall_viii"] = asm["dynCall_viii"];
    var dynCall_v = Module["dynCall_v"] = asm["dynCall_v"];
    var dynCall_viiiiii = Module["dynCall_viiiiii"] = asm["dynCall_viiiiii"];
    var dynCall_viiii = Module["dynCall_viiii"] = asm["dynCall_viiii"];
    Runtime.stackAlloc = Module["stackAlloc"];
    Runtime.stackSave = Module["stackSave"];
    Runtime.stackRestore = Module["stackRestore"];
    Runtime.establishStackSpace = Module["establishStackSpace"];
    Runtime.setTempRet0 = Module["setTempRet0"];
    Runtime.getTempRet0 = Module["getTempRet0"];
    Module["asm"] = asm;

    function ExitStatus(status) {
        this.name = "ExitStatus";
        this.message = "Program terminated with exit(" + status + ")";
        this.status = status;
    }

    ExitStatus.prototype = new Error;
    ExitStatus.prototype.constructor = ExitStatus;
    var initialStackTop;
    dependenciesFulfilled = function runCaller() {
        if (!Module["calledRun"]) run();
        if (!Module["calledRun"]) dependenciesFulfilled = runCaller;
    };
    Module["callMain"] = Module.callMain = function callMain(args) {
        args = args || [];
        ensureInitRuntime();
        var argc = args.length + 1;

        function pad() {
            for (var i = 0; i < 4 - 1; i++) {
                argv.push(0);
            }
        }

        var argv = [allocate(intArrayFromString(Module["thisProgram"]), "i8", ALLOC_NORMAL)];
        pad();
        for (var i = 0; i < argc - 1; i = i + 1) {
            argv.push(allocate(intArrayFromString(args[i]), "i8", ALLOC_NORMAL));
            pad();
        }
        argv.push(0);
        argv = allocate(argv, "i32", ALLOC_NORMAL);
        try {
            var ret = Module["_main"](argc, argv, 0);
            exit(ret, true);
        } catch (e) {
            if (e instanceof ExitStatus) {
            } else if (e == "SimulateInfiniteLoop") {
                Module["noExitRuntime"] = true;
            } else {
                var toLog = e;
                if (e && typeof e === "object" && e.stack) {
                    toLog = [e, e.stack];
                }
                Module.printErr("exception thrown: " + toLog);
                Module["quit"](1, e);
            }
        } finally {
        }
    };

    function run(args) {
        args = args || Module["arguments"];
        if (runDependencies > 0) {
            return
        }
        preRun();
        if (runDependencies > 0) return;
        if (Module["calledRun"]) return;

        function doRun() {
            if (Module["calledRun"]) return;
            Module["calledRun"] = true;
            if (ABORT) return;
            ensureInitRuntime();
            preMain();
            if (Module["onRuntimeInitialized"]) Module["onRuntimeInitialized"]();
            if (Module["_main"] && shouldRunNow) Module["callMain"](args);
            postRun();
        }

        if (Module["setStatus"]) {
            Module["setStatus"]("Running...");
            setTimeout((function () {
                setTimeout((function () {
                    Module["setStatus"]("");
                }), 1);
                doRun();
            }), 1);
        } else {
            doRun();
        }
    }

    Module["run"] = Module.run = run;

    function exit(status, implicit) {
        if (implicit && Module["noExitRuntime"]) {
            return
        }
        if (Module["noExitRuntime"]) ; else {
            ABORT = true;
            STACKTOP = initialStackTop;
            exitRuntime();
            if (Module["onExit"]) Module["onExit"](status);
        }
        if (ENVIRONMENT_IS_NODE) {
            process["exit"](status);
        }
        Module["quit"](status, new ExitStatus(status));
    }

    Module["exit"] = Module.exit = exit;
    var abortDecorators = [];

    function abort(what) {
        if (Module["onAbort"]) {
            Module["onAbort"](what);
        }
        if (what !== undefined) {
            Module.print(what);
            Module.printErr(what);
            what = JSON.stringify(what);
        } else {
            what = "";
        }
        ABORT = true;
        var extra = "\nIf this abort() is unexpected, build with -s ASSERTIONS=1 which can give more information.";
        var output = "abort(" + what + ") at " + stackTrace() + extra;
        if (abortDecorators) {
            abortDecorators.forEach((function (decorator) {
                output = decorator(output, what);
            }));
        }
        throw output
    }

    Module["abort"] = Module.abort = abort;
    if (Module["preInit"]) {
        if (typeof Module["preInit"] == "function") Module["preInit"] = [Module["preInit"]];
        while (Module["preInit"].length > 0) {
            Module["preInit"].pop()();
        }
    }
    var shouldRunNow = true;
    if (Module["noInitialRun"]) {
        shouldRunNow = false;
    }
    Module["noExitRuntime"] = true;
    run();

    var crunch = Module;

    // Modified from texture-tester
    // See:
    //     https://github.com/toji/texture-tester/blob/master/js/webgl-texture-util.js
    //     http://toji.github.io/texture-tester/

    /**
     * @license
     *
     * Copyright (c) 2014, Brandon Jones. All rights reserved.
     *
     * Redistribution and use in source and binary forms, with or without modification,
     * are permitted provided that the following conditions are met:
     *
     *  * Redistributions of source code must retain the above copyright notice, this
     *  list of conditions and the following disclaimer.
     *  * Redistributions in binary form must reproduce the above copyright notice,
     *  this list of conditions and the following disclaimer in the documentation
     *  and/or other materials provided with the distribution.
     *
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
     * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
     * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
     * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
     * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
     * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
     * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
     * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
     * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
     * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
     */

        // Taken from crnlib.h
    var CRN_FORMAT = {
            cCRNFmtInvalid: -1,

            cCRNFmtDXT1: 0,
            // cCRNFmtDXT3 is not currently supported when writing to CRN - only DDS.
            cCRNFmtDXT3: 1,
            cCRNFmtDXT5: 2,

            // Crunch supports more formats than this, but we can't use them here.
        };

    // Mapping of Crunch formats to DXT formats.
    var DXT_FORMAT_MAP = {};
    DXT_FORMAT_MAP[CRN_FORMAT.cCRNFmtDXT1] = PixelFormat$1.RGB_DXT1;
    DXT_FORMAT_MAP[CRN_FORMAT.cCRNFmtDXT3] = PixelFormat$1.RGBA_DXT3;
    DXT_FORMAT_MAP[CRN_FORMAT.cCRNFmtDXT5] = PixelFormat$1.RGBA_DXT5;

    var dst;
    var dxtData;
    var cachedDstSize = 0;

    // Copy an array of bytes into or out of the emscripten heap.
    function arrayBufferCopy(src, dst, dstByteOffset, numBytes) {
        var i;
        var dst32Offset = dstByteOffset / 4;
        var tail = numBytes % 4;
        var src32 = new Uint32Array(src.buffer, 0, (numBytes - tail) / 4);
        var dst32 = new Uint32Array(dst.buffer);
        for (i = 0; i < src32.length; i++) {
            dst32[dst32Offset + i] = src32[i];
        }
        for (i = numBytes - tail; i < numBytes; i++) {
            dst[dstByteOffset + i] = src[i];
        }
    }

    /**
     * @private
     */
    function transcodeCRNToDXT(arrayBuffer, transferableObjects) {
        // Copy the contents of the arrayBuffer into emscriptens heap.
        var srcSize = arrayBuffer.byteLength;
        var bytes = new Uint8Array(arrayBuffer);
        var src = crunch._malloc(srcSize);
        arrayBufferCopy(bytes, crunch.HEAPU8, src, srcSize);

        // Determine what type of compressed data the file contains.
        var crnFormat = crunch._crn_get_dxt_format(src, srcSize);
        var format = DXT_FORMAT_MAP[crnFormat];
        if (!when.defined(format)) {
            throw new RuntimeError.RuntimeError("Unsupported compressed format.");
        }

        // Gather basic metrics about the DXT data.
        var levels = crunch._crn_get_levels(src, srcSize);
        var width = crunch._crn_get_width(src, srcSize);
        var height = crunch._crn_get_height(src, srcSize);

        // Determine the size of the decoded DXT data.
        var dstSize = 0;
        var i;
        for (i = 0; i < levels; ++i) {
            dstSize += PixelFormat$1.compressedTextureSizeInBytes(
                format,
                width >> i,
                height >> i
            );
        }

        // Allocate enough space on the emscripten heap to hold the decoded DXT data
        // or reuse the existing allocation if a previous call to this function has
        // already acquired a large enough buffer.
        if (cachedDstSize < dstSize) {
            if (when.defined(dst)) {
                crunch._free(dst);
            }
            dst = crunch._malloc(dstSize);
            dxtData = new Uint8Array(crunch.HEAPU8.buffer, dst, dstSize);
            cachedDstSize = dstSize;
        }

        // Decompress the DXT data from the Crunch file into the allocated space.
        crunch._crn_decompress(src, srcSize, dst, dstSize, 0, levels);

        // Release the crunch file data from the emscripten heap.
        crunch._free(src);

        // Mipmaps are unsupported, so copy the level 0 texture
        // When mipmaps are supported, a copy will still be necessary as dxtData is a view on the heap.
        var length = PixelFormat$1.compressedTextureSizeInBytes(format, width, height);

        // Get a copy of the 0th mip level. dxtData will exceed length when there are more mip levels.
        // Equivalent to dxtData.slice(0, length), which is not supported in IE11
        var level0DXTDataView = dxtData.subarray(0, length);
        var level0DXTData = new Uint8Array(length);
        level0DXTData.set(level0DXTDataView, 0);

        transferableObjects.push(level0DXTData.buffer);
        return new CompressedTextureBuffer(format, width, height, level0DXTData);
    }

    var transcodeCRNToDXT$1 = createTaskProcessorWorker(transcodeCRNToDXT);

    return transcodeCRNToDXT$1;

});
//# sourceMappingURL=transcodeCRNToDXT.js.map
