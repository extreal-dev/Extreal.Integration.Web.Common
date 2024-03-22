type Pointer = number;

type ActionMethod = (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => void;
type FunctionMethod = (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => string;
type CallbackMethod = (namePtr: Pointer, callbackPtr: Pointer) => void;
type BindMethod = ActionMethod | FunctionMethod | CallbackMethod;

type Module = {
    _malloc: (size: number) => Pointer;
    _free: (ptr: Pointer) => void;
    dynCall_vii: (funcPtr: Pointer, buf1: Pointer, buf2: Pointer) => void;
};

type HelperConfig = {
    isDebug: boolean;
};

type Texture = {
    name: number;
}

type GL = {
    textures: Texture[];
};

type WebGLShader = {
}

type WebGLBuffer = {
}

type WebGLFramebuffer = {
}

type Canvas = {
    width: number,
    height: number,
}

type texImage2DType = {
    (texType: number, num: number, colorFormat1: number, colorFormat2: number, type: number, element: HTMLMediaElement): void;
    (texType: number, num: number, colorFormat1: number, width: number, height: number, border: number, colorFormat2: number, type: number, element: null): void;
}

type GLctx = {
    TEXTURE_2D: number;
    UNPACK_FLIP_Y_WEBGL: number;
    TEXTURE_WRAP_S: number;
    TEXTURE_WRAP_T: number;
    TEXTURE_MIN_FILTER: number;
    CLAMP_TO_EDGE: number;
    LINEAR: number;
    RGBA: number;
    UNSIGNED_BYTE: number;
    VERTEX_SHADER: number;
    FRAGMENT_SHADER: number;
    TRIANGLES: number;
    FLOAT: number;
    ARRAY_BUFFER: number;
    STATIC_DRAW: number;
    FRAMEBUFFER: number;
    COLOR_ATTACHMENT0: number;
    deleteTexture: (tex: Texture) => void;
    createTexture: () => Texture;
    bindTexture: (texType: number, tex: Texture) => void;
    pixelStorei: (flipDir: number, isFlipped: boolean) => void;
    texParameteri: (texType: number, method: number, op: number) => void;
    texImage2D: texImage2DType;
    useProgram: (program: WebGLProgram) => void;
    bindFramebuffer: (target: number, framebuffer: WebGLFramebuffer | null) => void;
    createShader: (type: number) => WebGLShader;
    shaderSource: (shader: WebGLShader, source: string) => void;
    compileShader: (shader: WebGLShader) => void;
    createProgram: () => WebGLProgram;
    attachShader: (program: WebGLProgram, shader: WebGLShader) => void;
    linkProgram: (program: WebGLProgram) => void;
    drawArrays: (mode: number, first: number, count: number) => void;
    viewport: (x: number, y: number, width: number, height: number) => void;
    enableVertexAttribArray: (index: number) => void;
    vertexAttribPointer: (index: number, size: number, type: number, normalized: boolean, stride: number, offset: number) => void;
    getAttribLocation: (program: WebGLProgram, name: string) => number;
    createBuffer: () => WebGLBuffer;
    bindBuffer: (target: number, buffer: WebGLBuffer) => void;
    bufferData: (target: number, srcData: ArrayBuffer, usage: number) => void;
    createFramebuffer: () => WebGLFramebuffer;
    framebufferTexture2D: (target: number, attachment: number, textarget: number, texture: Texture, level: number) => void;
    canvas: Canvas;
}


type Helper = {
    Module: Module;
    lengthBytesUTF8: (str: string) => number;
    stringToUTF8: (str: string, buf: Pointer, size: number) => void;
    UTF8ToString: (ptr: Pointer) => string;
    GL: GL;
    GLctx: GLctx;
};

/**
 * Debug or not.
 */
let isDebug: boolean;

let helper: Helper;
const boundMethods = new Map<string, BindMethod>();

declare global {
    // rome-ignore lint/style/noVar: To call from Unity's jslib
    var __getNop: (helperObj: Helper, getGLctx: () => GLctx) => void;
    // rome-ignore lint/style/noVar: To call from Unity's jslib
    var __getMethod: (name: string) => BindMethod;
}

globalThis["__getNop"] = (helperObj, getGLctx) => {
    return (jsonConfigPtr: Pointer) => {
        helperObj.GLctx = getGLctx();
        helper = helperObj;
        isDebug = (JSON.parse(ptrToStr(jsonConfigPtr)) as HelperConfig).isDebug;
        console.log(`helper: isDebug=${isDebug}`);
    };
};

globalThis["__getMethod"] = (name) => {
    const method = boundMethods.get(name);
    if (!method) {
        throw new Error(`bound method not found. name=${name}`);
    }
    return method;
};

const bindMethod = (name: string, method: BindMethod) => {
    boundMethods.set(name, method);
    return;
};

const ptrToStr = (ptr: Pointer): string => helper.UTF8ToString(ptr);

const strToPtr = (str: string): Pointer => {
    const size = helper.lengthBytesUTF8(str) + 1;
    const ptr = helper.Module._malloc(size);
    helper.stringToUTF8(str, ptr, size);
    return ptr;
};

const callbackToUnity = (callbackPtr: Pointer, str1: string, str2: string): void => {
    const ptr1 = strToPtr(str1);
    const ptr2 = strToPtr(str2);
    helper.Module.dynCall_vii(callbackPtr, ptr1, ptr2);
    helper.Module._free(ptr1);
    helper.Module._free(ptr2);
};

type Action = (str1: string, str2: string) => void;
type Function = (str1: string, str2: string) => string;
type Callback = (str1: string, str2: string) => void;

const actions = new Map<string, Action>();
const functions = new Map<string, Function>();
const callbacks = new Map<string, Callback>();

const traceLogSuppressedNames = new Set<string>();

const UNUSED = "";

bindMethod("CallAction", (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => {
    const name = ptrToStr(namePtr);
    const action = actions.get(name);
    if (!action) {
        throw new Error(`A action to call not found. name=${name}`);
    }
    const strParam1 = strParamPtr1 ? ptrToStr(strParamPtr1) : UNUSED;
    const strParam2 = strParamPtr2 ? ptrToStr(strParamPtr2) : UNUSED;
    if (isDebug && !traceLogSuppressedNames.has(name)) {
        console.log(`call action: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    action(strParam1, strParam2);
});

bindMethod("CallFunction", (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => {
    const name = ptrToStr(namePtr);
    const func = functions.get(name);
    if (!func) {
        throw new Error(`A function to call not found. name=${name}`);
    }
    const strParam1 = strParamPtr1 ? ptrToStr(strParamPtr1) : UNUSED;
    const strParam2 = strParamPtr2 ? ptrToStr(strParamPtr2) : UNUSED;
    if (isDebug && !traceLogSuppressedNames.has(name)) {
        console.log(`call function: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    return strToPtr(func(strParam1, strParam2));
});

bindMethod("AddCallback", (namePtr: Pointer, callbackPtr: Pointer) => {
    const name = ptrToStr(namePtr);
    if (isDebug) {
        console.log(`add callback: name=${name}`);
    }
    callbacks.set(name, (str1, str2) => {
        callbackToUnity(callbackPtr, str1, str2);
    });
});

/**
 * Adds a function without a return value.
 * 
 * @param name - Target
 * @param action - Function
 */
const addAction = (name: string, action: Action, isSuppressTraceLog: boolean = false) => {
    actions.set(name, action);
    if (isSuppressTraceLog) {
        suppressTraceLog(name);
    }
};

/**
 * Adds a function with a return value.
 * 
 * @param name - Target
 * @param func - Function
 */
const addFunction = (name: string, func: Function, isSuppressTraceLog: boolean = false) => {
    functions.set(name, func);
    if (isSuppressTraceLog) {
        suppressTraceLog(name);
    }
};

/**
 * Callbacks.
 * 
 * @param name - Target
 * @param strParam1 - First string parameter
 * @param strParam2 - Second string parameter
 */
const callback = (name: string, strParam1?: string, strParam2?: string, isSuppressTraceLog: boolean = false) => {
    const cb = callbacks.get(name);
    if (!cb) {
        throw new Error(`A callback to call not found. name=${name}`);
    }
    if (isDebug && !isSuppressTraceLog) {
        console.log(`call callback: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    cb(strParam1 ?? UNUSED, strParam2 ?? UNUSED);
};

let s2lProgram: WebGLProgram | null = null;
let s2lVertexPositionNDC: number | null = null;
let s2lVBO: WebGLBuffer | null = null;
let s2lFBO: WebGLFramebuffer | null = null;
let s2lTexture: Texture | null = null;
let prevVideoWidth: number = 0;
let prevVideoHeight: number = 0;

const createS2lProgram = () => {
    const vertexShaderCode = `
        precision lowp float;
        attribute vec2 vertexPositionNDC;
        varying vec2 vTexCoords;
        const vec2 scale = vec2(0.5, 0.5);
        void main() {
            vTexCoords = vertexPositionNDC * scale + scale;
            gl_Position = vec4(vertexPositionNDC, 0.0, 1.0);
        }`;

    const fragmentShaderCode = `
        precision mediump float;
        uniform sampler2D colorMap;
        varying vec2 vTexCoords;
        vec4 toLinear(vec4 sRGB) {
            vec3 c = sRGB.rgb;
            return vec4(c * (c * (c * 0.305306011 + 0.682171111) + 0.012522878), sRGB.a);
        }
        void main() {
            gl_FragColor = toLinear(texture2D(colorMap, vTexCoords));
        }`;

    const vertexShader = helper.GLctx.createShader(helper.GLctx.VERTEX_SHADER);
    helper.GLctx.shaderSource(vertexShader, vertexShaderCode);
    helper.GLctx.compileShader(vertexShader);

    const fragmentShader = helper.GLctx.createShader(helper.GLctx.FRAGMENT_SHADER);
    helper.GLctx.shaderSource(fragmentShader, fragmentShaderCode);
    helper.GLctx.compileShader(fragmentShader);

    s2lProgram = helper.GLctx.createProgram();
    helper.GLctx.attachShader(s2lProgram, vertexShader);
    helper.GLctx.attachShader(s2lProgram, fragmentShader);
    helper.GLctx.linkProgram(s2lProgram);

    s2lVertexPositionNDC = helper.GLctx.getAttribLocation(s2lProgram, "vertexPositionNDC");
};

const createS2lVBO = () => {
    s2lVBO = helper.GLctx.createBuffer();
    helper.GLctx.bindBuffer(helper.GLctx.ARRAY_BUFFER, s2lVBO);

    var verts = [
        1.0,  1.0,
        -1.0,  1.0,
        -1.0, -1.0,
        -1.0, -1.0,
        1.0, -1.0,
        1.0,  1.0
    ];
    helper.GLctx.bufferData(helper.GLctx.ARRAY_BUFFER, new Float32Array(verts), helper.GLctx.STATIC_DRAW);
};

const createTexture = () => {
    const texture = helper.GLctx.createTexture();
    helper.GLctx.bindTexture(helper.GLctx.TEXTURE_2D, texture);
    helper.GLctx.texParameteri(helper.GLctx.TEXTURE_2D, helper.GLctx.TEXTURE_WRAP_S, helper.GLctx.CLAMP_TO_EDGE);
    helper.GLctx.texParameteri(helper.GLctx.TEXTURE_2D, helper.GLctx.TEXTURE_WRAP_T, helper.GLctx.CLAMP_TO_EDGE);
    helper.GLctx.texParameteri(helper.GLctx.TEXTURE_2D, helper.GLctx.TEXTURE_MIN_FILTER, helper.GLctx.LINEAR);
    return texture;
};

/**
 * Updates texture.
 *
 * @param element - HTMLVideoElement
 * @param textureId - Native texture ID of Unity
 */
const updateTexture = (element: HTMLVideoElement, textureId: number) => {
    const prevTexture = helper.GL.textures[textureId];
    helper.GLctx.deleteTexture(prevTexture);
    
    helper.GLctx.pixelStorei(helper.GLctx.UNPACK_FLIP_Y_WEBGL, true);

    const texture = createTexture();
    texture.name = textureId;
    helper.GL.textures[textureId] = texture;
    helper.GLctx.texImage2D(helper.GLctx.TEXTURE_2D, 0, helper.GLctx.RGBA, element.videoWidth, element.videoHeight, 0, helper.GLctx.RGBA, helper.GLctx.UNSIGNED_BYTE, null);
    
    if (element.readyState >= element.HAVE_ENOUGH_DATA) {
        if (s2lTexture === null || prevVideoHeight !== element.videoHeight || prevVideoWidth !== element.videoWidth) {
            s2lTexture = createTexture();
            prevVideoHeight = element.videoHeight;
            prevVideoWidth = element.videoWidth;
        } else {
            helper.GLctx.bindTexture(helper.GLctx.TEXTURE_2D, s2lTexture);
        }
        helper.GLctx.texImage2D(helper.GLctx.TEXTURE_2D, 0, helper.GLctx.RGBA, helper.GLctx.RGBA, helper.GLctx.UNSIGNED_BYTE, element);

        if (s2lProgram === null) {
            createS2lProgram();
        }
        if (s2lVBO === null) {
            createS2lVBO();
        }
        if (s2lFBO === null) {
            s2lFBO = helper.GLctx.createFramebuffer();
        }

        if (s2lProgram !== null && s2lVertexPositionNDC !== null && s2lVBO !== null) {
            helper.GLctx.bindFramebuffer(helper.GLctx.FRAMEBUFFER, s2lFBO);
            helper.GLctx.framebufferTexture2D(helper.GLctx.FRAMEBUFFER, helper.GLctx.COLOR_ATTACHMENT0, helper.GLctx.TEXTURE_2D, texture, 0);

            helper.GLctx.viewport(0, 0, element.videoWidth, element.videoHeight);
            helper.GLctx.useProgram(s2lProgram);
            helper.GLctx.bindBuffer(helper.GLctx.ARRAY_BUFFER, s2lVBO);
            helper.GLctx.enableVertexAttribArray(s2lVertexPositionNDC);
            helper.GLctx.vertexAttribPointer(s2lVertexPositionNDC, 2, helper.GLctx.FLOAT, false, 0, 0);
            helper.GLctx.drawArrays(helper.GLctx.TRIANGLES, 0, 6);

            helper.GLctx.viewport(0, 0, helper.GLctx.canvas.width, helper.GLctx.canvas.height);
            helper.GLctx.bindFramebuffer(helper.GLctx.FRAMEBUFFER, null);
        }
    }
    helper.GLctx.pixelStorei(helper.GLctx.UNPACK_FLIP_Y_WEBGL, false);
};

/**
 * Waits until the condition is satisfied.
 * 
 * @param condition - Condition
 * @param cancel - Function to determine whether to cancel
 * @param interval - Interval to be checked(milliseconds)
 * @returns Promise
 */
const waitUntil = (condition: () => boolean, cancel: () => boolean, interval = 100) => {
    return new Promise<void>((resolve, reject) => {
        const checkCondition = () => {
            if (condition() || cancel()) {
                resolve();
            } else {
                setTimeout(checkCondition, interval);
            }
        };
        checkCondition();
    });
};

/**
 * Determines if it is an AsyncFunction.
 * 
 * @param func - Function
 */
const isAsync = (func: object) => {
    return typeof func === "function" && Object.prototype.toString.call(func) === "[object AsyncFunction]";
};

const suppressTraceLog = (name: string) => {
    traceLogSuppressedNames.add(name);
}

export { addAction, addFunction, callback, updateTexture, isDebug, waitUntil, isAsync };
