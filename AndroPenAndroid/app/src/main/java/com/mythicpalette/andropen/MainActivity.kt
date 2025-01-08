package com.mythicpalette.andropen

import android.annotation.SuppressLint
import android.content.Intent
import android.os.Bundle
import android.view.MotionEvent
import android.view.View
import android.widget.ImageButton
import androidx.appcompat.app.AppCompatActivity
import com.mythicpalette.andropen.helpers.ConnectionState
import com.mythicpalette.andropen.helpers.SocketHandler
import com.mythicpalette.andropen.helpers.SocketStateListener

class MainActivity : AppCompatActivity() {
    private var socketHandler: SocketHandler = SocketHandler();

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
        socketHandler.connectToSocket(this);
        findViewById<View>(R.id.reconnect_button).setOnTouchListener{ v, ev ->
            when (ev.actionMasked) {
                MotionEvent.ACTION_UP -> {
                    socketHandler.connectToSocket(this);
                    true
                }
                else -> {
                    true
                }
            }
        }
        findViewById<TouchInputView>(R.id.drawArea).socketHandler = this.socketHandler
        findViewById<ImageButton>(R.id.show_settings_button).setOnClickListener{ _ ->
            val intent = Intent(this, SettingsActivity::class.java)
            startActivity(intent)
            true
        }
    }
}