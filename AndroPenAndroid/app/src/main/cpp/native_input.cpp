#include <jni.h>
#include <android/input.h>
#include <vector>
#include <unistd.h> // For sleep

struct PointerInfo {
    int32_t pointerId;
    int32_t toolType;
    float x;
    float y;
    float pressure;
    float tiltX;
    float tiltY;
};

// Helper function to retrieve the number of pointers from the input event
int32_t getPointerCount(AInputEvent* event) {
    return AMotionEvent_getPointerCount(event);
}

// Helper function to retrieve pointer info from the input event
PointerInfo getPointerInfo(AInputEvent* event, int pointerIndex) {
    PointerInfo pointerInfo;

    pointerInfo.pointerId = AMotionEvent_getPointerId(event, pointerIndex);
    pointerInfo.toolType = AMotionEvent_getToolType(event, pointerIndex);
    pointerInfo.x = AMotionEvent_getX(event, pointerIndex);
    pointerInfo.y = AMotionEvent_getY(event, pointerIndex);
    pointerInfo.pressure = AMotionEvent_getPressure(event, pointerIndex);

    // For tilt, you might use precision as a proxy
    pointerInfo.tiltX = AMotionEvent_getXPrecision(event);
    pointerInfo.tiltY = AMotionEvent_getYPrecision(event);

    return pointerInfo;
}

// This function will continuously poll the pointers until stopped (using a thread or loop)
extern "C"
JNIEXPORT jobjectArray JNICALL
Java_com_mythicpalette_andropen_helpers_NativeInput_getPointerInfo(JNIEnv *env, jobject /*this*/) {
    // Open an input device (this can be a generic touchscreen device)
    AInputEvent* event = nullptr;  // You need a way to get the actual event from the device

    std::vector<PointerInfo> pointerInfoList;

    while (true) {
        // Poll the input events (you need the event polling mechanism here)
        // This is a simplified representation:
        event = getNextInputEvent();  // This function needs to be implemented for continuous event polling

        if (event != nullptr) {
            int32_t pointerCount = getPointerCount(event);
            for (int32_t i = 0; i < pointerCount; ++i) {
                PointerInfo pointer = getPointerInfo(event, i);
                pointerInfoList.push_back(pointer);
            }
        }

        // Create an array of PointerInfo to return
        jclass pointerInfoClass = env->FindClass("com/mythicpalette/andropen/helpers/PointerInfo");
        jobjectArray pointerInfoArray = env->NewObjectArray(pointerInfoList.size(), pointerInfoClass, nullptr);

        // Loop to fill the array
        for (size_t i = 0; i < pointerInfoList.size(); ++i) {
            PointerInfo pointer = pointerInfoList[i];
            jobject pointerInfoObject = env->AllocObject(pointerInfoClass);

            // Set the fields of the PointerInfo object
            jfieldID pointerIdField = env->GetFieldID(pointerInfoClass, "pointerId", "I");
            jfieldID toolTypeField = env->GetFieldID(pointerInfoClass, "toolType", "I");
            jfieldID xField = env->GetFieldID(pointerInfoClass, "x", "F");
            jfieldID yField = env->GetFieldID(pointerInfoClass, "y", "F");
            jfieldID pressureField = env->GetFieldID(pointerInfoClass, "pressure", "F");
            jfieldID tiltXField = env->GetFieldID(pointerInfoClass, "tiltX", "F");
            jfieldID tiltYField = env->GetFieldID(pointerInfoClass, "tiltY", "F");

            env->SetIntField(pointerInfoObject, pointerIdField, pointer.pointerId);
            env->SetIntField(pointerInfoObject, toolTypeField, pointer.toolType);
            env->SetFloatField(pointerInfoObject, xField, pointer.x);
            env->SetFloatField(pointerInfoObject, yField, pointer.y);
            env->SetFloatField(pointerInfoObject, pressureField, pointer.pressure);
            env->SetFloatField(pointerInfoObject, tiltXField, pointer.tiltX);
            env->SetFloatField(pointerInfoObject, tiltYField, pointer.tiltY);

            env->SetObjectArrayElement(pointerInfoArray, i, pointerInfoObject);
        }

        // Sleep for a short time to allow other tasks to run
        usleep(10000);  // Sleep for 10 milliseconds (100Hz polling rate)
    }

    return pointerInfoArray;
}
