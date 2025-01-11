package com.mythicpalette.andropen.helpers

data class NativePointerInfo(
    val pointerId: Int,
    val toolType: Int,
    val x: Float,
    val y: Float,
    val pressure: Float,
    val tiltX: Float,
    val tiltY: Float
)
object NativeInput {
    init {
        System.loadLibrary("native_input")
    }

    external fun getPointerInfo(): Array<NativePointerInfo>
}