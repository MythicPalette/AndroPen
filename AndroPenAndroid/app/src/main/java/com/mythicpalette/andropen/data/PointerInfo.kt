package com.mythicpalette.andropen.data

import android.view.MotionEvent
import java.nio.ByteBuffer
import java.nio.ByteOrder

data class PointerInfo(
    var pointerId: Int, // Unique pointer identifier (e.g., a finger or stylus)
    var eventType: Int, // Event type (DOWN, MOVE, UP, HOVER_ENTER, HOVER_MOVE, HOVER_EXIT)
    var x: Float, // X coordinate of the pointer
    var y: Float, // Y coordinate of the pointer
    var pressure: Float = 0f, // Pressure applied (0.0 - 1.0)
    var tiltX: Float = 0f, // Tilt in X direction for pen input
    var tiltY: Float = 0f, // Tilt in Y direction for pen input
    var timeStamp: Long, // Time of the event (milliseconds)
    var pointerType: Int = MotionEvent.TOOL_TYPE_STYLUS, // Type of pointer: PEN, TOUCH, etc.
    var velocityX: Float = 0f, // Velocity of movement along the X axis
    var velocityY: Float = 0f,  // Velocity of movement along the Y axis
    var viewWidth: Int = 0,
    var viewHeight: Int = 0
)

fun PointerInfo.serialize(): ByteArray {
    // Calculate the total size of the byte array
    val totalSize = 56 // The total size of the packet

    // Create a ByteBuffer with the required size
    val buffer = ByteBuffer.allocate(totalSize).order(ByteOrder.LITTLE_ENDIAN)

    // Serialize the data
    buffer.putInt(pointerId)
    buffer.putInt(eventType)
    buffer.putFloat(x)
    buffer.putFloat(y)
    buffer.putFloat(pressure)
    buffer.putFloat(tiltX)
    buffer.putFloat(tiltY)
    buffer.putLong(timeStamp)
    buffer.putInt(pointerType)
    buffer.putFloat(velocityX)
    buffer.putFloat(velocityY)
    buffer.putInt(viewWidth)
    buffer.putInt(viewHeight)

    // Return the byte array
    return buffer.array()
}