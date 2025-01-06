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

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        socketHandler.addListener(object: SocketStateListener {
            override fun onConnectionStateChanged(state: ConnectionState) {
                findViewById<SignalButton>(R.id.reconnect_button).connected = state.Connected
                if ( state.Connected ) println("Connected")
                else println("Disconnected")
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
        findViewById<TouchInputView>(R.id.drawArea).socketHandler = this.socketHandler
    }
}