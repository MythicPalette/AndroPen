package com.mythicpalette.andropen

import android.annotation.SuppressLint
import android.os.Bundle
import android.view.GestureDetector
import android.view.MotionEvent
import android.view.ScaleGestureDetector
import android.view.View
import android.widget.Toast
import androidx.activity.ComponentActivity
import com.mythicpalette.andropen.data.EventType
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.PointerType
import com.mythicpalette.andropen.data.serialize
import com.mythicpalette.andropen.helpers.ConnectionState
import com.mythicpalette.andropen.helpers.SocketHandler
import com.mythicpalette.andropen.helpers.SocketStateListener
import java.nio.ByteBuffer
import java.nio.ByteOrder

private var MESSAGE_TAP: Int = 1

class MainActivity : ComponentActivity() {

    private var socketHandler: SocketHandler = SocketHandler("192.168.0.13", 18998 );
    private lateinit var gestureDetector: GestureDetector
    private lateinit var scaleGestureDetector: ScaleGestureDetector

    private var penHovering: Boolean = false;
    private var penTouching: Boolean = false;

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        socketHandler.addListener(object: SocketStateListener {
            override fun onConnectionStateChanged(state: ConnectionState) {
                //TODO("Not yet implemented")
                if ( state.Connected ) { // connected
                    println("Connected")
                    findViewById<View>(R.id.reconnect_button).setBackgroundColor(getColor(R.color.green))
                } else { // disconnected
                    println("Disconnected")
                    findViewById<View>(R.id.reconnect_button).setBackgroundColor(getColor(R.color.red))
                }
            }
        })
        socketHandler.connectToSocket();

        findViewById<View>(R.id.reconnect_button).setOnTouchListener{ v, ev ->
            when (ev.actionMasked) {
                MotionEvent.ACTION_UP -> {
                    socketHandler.connectToSocket();
                    true
                }
                else -> {
                    true
                }
            }
        }
        findViewById<View>(R.id.drawArea).setOnHoverListener { v, ev ->
            // Send the pointer count.
            val buffer = ByteBuffer.allocate(4 + (68*ev.pointerCount)).order(ByteOrder.LITTLE_ENDIAN)
            buffer.putInt(ev.pointerCount);

            val pi = PointerInfo(
                pointerId = ev.getPointerId(0),
                eventType = EventType.HOVER_ENTER,
                x = ev.getX(0),
                y = ev.getY(0),
                pressure = 0f,
                pointerType = PointerType.PEN,
                tiltX = ev.getAxisValue(MotionEvent.AXIS_TILT, 0),
                tiltY = ev.getAxisValue(MotionEvent.AXIS_TILT, 0),
                timeStamp = System.currentTimeMillis(),
                viewWidth = v.width,
                viewHeight = v.height
            );

            when (ev.action) {
                MotionEvent.ACTION_HOVER_ENTER -> penHovering = true;
                MotionEvent.ACTION_HOVER_MOVE -> pi.eventType = EventType.HOVER_MOVE
                MotionEvent.ACTION_HOVER_EXIT -> {
                    penHovering = false;
                    pi.eventType = EventType.HOVER_EXIT
                }
            }

            buffer.put(pi.serialize())

            socketHandler.send(buffer.array())
            true;
        }
        findViewById<View>(R.id.drawArea).setOnTouchListener { v, ev ->
            val infos: MutableList<PointerInfo> = mutableListOf()
            val action = ev.actionMasked
            val stamp = System.currentTimeMillis();
            val viewWidth = v.width;
            val viewHeight = v.height;

            // Iterate over all pointers to handle pen input (pressure, movement, etc.)
            for (i in 0 until ev.pointerCount) {
                val pointerId = ev.getPointerId(i)
                val toolType = ev.getToolType(i)  // Check if it's a pen or finger

                val pi = PointerInfo(
                    pointerId = pointerId,
                    eventType = EventType.DOWN,
                    x = ev.getX(i),
                    y = ev.getY(i),
                    pressure = 0f,
                    pointerType = PointerType.PEN,
                    tiltX = 0f,
                    tiltY = 0f,
                    timeStamp = stamp,
                    viewWidth = viewWidth,
                    viewHeight = viewHeight
                );

                // Check if the input device is a pen or a finger
                when (toolType) {
                    MotionEvent.TOOL_TYPE_STYLUS -> {
                        pi.pressure = ev.getPressure(i)
                        pi.tiltX = ev.getAxisValue(MotionEvent.AXIS_TILT, i);
                        pi.tiltY = ev.getAxisValue(MotionEvent.AXIS_TILT, i)
                        // Pen input detected (e.g., S-Pen, stylus)
                        when (action) {
                            MotionEvent.ACTION_DOWN -> {
                                penTouching = true
                                println("Pen Down")
                            };
                            MotionEvent.ACTION_MOVE -> pi.eventType = EventType.MOVE
                            MotionEvent.ACTION_UP -> {
                                pi.eventType = EventType.UP
                                penTouching = false
                                println("Pen Up");
                            }
                            MotionEvent.ACTION_POINTER_UP -> {
                                pi.eventType = EventType.POINTER_UP
                                penTouching = false
                            }
                        }
                    }

                    MotionEvent.TOOL_TYPE_FINGER -> {
                        if ( penHovering || penTouching )
                            continue

                        pi.pointerType = PointerType.TOUCH;
                        // Finger touch detected (tap or gesture)
                        when (action) {
                            MotionEvent.ACTION_POINTER_DOWN -> pi.eventType =
                                EventType.POINTER_DOWN

                            MotionEvent.ACTION_UP -> pi.eventType = EventType.UP
                            MotionEvent.ACTION_POINTER_UP -> pi.eventType =
                                EventType.POINTER_UP;
                            MotionEvent.ACTION_MOVE -> pi.eventType = EventType.MOVE;
                        }
                    }

                    else -> {}
                }
                infos.add(pi)
            }

            // Create the byte buffer.
            val buffer =
                ByteBuffer.allocate(4 + (68 * infos.size)).order(ByteOrder.LITTLE_ENDIAN)

            // Serialize the number of touch events to send.
            buffer.putInt(infos.size);

            // Serialize the touch data.
            for (pi in infos)
                buffer.put(pi.serialize())

            // Send the data.
            socketHandler.send(buffer.array())
            true
        }
    }
}