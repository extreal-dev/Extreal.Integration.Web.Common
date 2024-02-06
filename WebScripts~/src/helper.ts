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
    deleteTexture: (tex: Texture) => void;
    createTexture: () => Texture;
    bindTexture: (texType: number, tex: Texture) => void;
    pixelStorei: (flipDir: number, isFlipped: boolean) => void;
    texParameteri: (texType: number, method: number, op: number) => void;
    texImage2D: (texType: number, num: number, colorFormat1: number, colorFormat2: number, type: number, element: HTMLMediaElement) => void;
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

const UNUSED = "";

bindMethod("CallAction", (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => {
    const name = ptrToStr(namePtr);
    const action = actions.get(name);
    if (!action) {
        throw new Error(`A action to call not found. name=${name}`);
    }
    const strParam1 = strParamPtr1 ? ptrToStr(strParamPtr1) : UNUSED;
    const strParam2 = strParamPtr2 ? ptrToStr(strParamPtr2) : UNUSED;
    if (isDebug) {
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
    if (isDebug) {
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
const addAction = (name: string, action: Action) => {
    actions.set(name, action);
};

/**
 * Adds a function with a return value.
 * 
 * @param name - Target
 * @param func - Function
 */
const addFunction = (name: string, func: Function) => {
    functions.set(name, func);
};

/**
 * Callbacks.
 * 
 * @param name - Target
 * @param strParam1 - First string parameter
 * @param strParam2 - Second string parameter
 */
const callback = (name: string, strParam1?: string, strParam2?: string) => {
    const cb = callbacks.get(name);
    if (!cb) {
        throw new Error(`A callback to call not found. name=${name}`);
    }
    if (isDebug) {
        console.log(`call callback: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    cb(strParam1 ?? UNUSED, strParam2 ?? UNUSED);
};

/**
 * Updates texture.
 *
 * @param element - HTMLMediaElement
 * @param textureId - Native texture ID of Unity
 */
const updateTexture = (element: HTMLMediaElement, textureId: number) => {
  const prevTexture = helper.GL.textures[textureId];
  helper.GLctx.deleteTexture(prevTexture);

  const texture = helper.GLctx.createTexture();
  texture.name = textureId;
  helper.GL.textures[textureId] = texture;

  helper.GLctx.bindTexture(helper.GLctx.TEXTURE_2D, helper.GL.textures[textureId]);
  helper.GLctx.pixelStorei(helper.GLctx.UNPACK_FLIP_Y_WEBGL, true);
  helper.GLctx.texParameteri(helper.GLctx.TEXTURE_2D, helper.GLctx.TEXTURE_WRAP_S, helper.GLctx.CLAMP_TO_EDGE);
  helper.GLctx.texParameteri(helper.GLctx.TEXTURE_2D, helper.GLctx.TEXTURE_WRAP_T, helper.GLctx.CLAMP_TO_EDGE);
  helper.GLctx.texParameteri(helper.GLctx.TEXTURE_2D, helper.GLctx.TEXTURE_MIN_FILTER, helper.GLctx.LINEAR);
  if (element.readyState >= element.HAVE_ENOUGH_DATA) {
    helper.GLctx.texImage2D(helper.GLctx.TEXTURE_2D, 0, helper.GLctx.RGBA, helper.GLctx.RGBA, helper.GLctx.UNSIGNED_BYTE, element);
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

export { addAction, addFunction, callback, updateTexture, isDebug, waitUntil, isAsync };
