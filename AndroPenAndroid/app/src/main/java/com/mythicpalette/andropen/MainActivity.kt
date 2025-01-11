package com.mythicpalette.andropen

import android.annotation.SuppressLint
import android.content.Intent
import android.os.Bundle
import android.view.MotionEvent
import android.view.View
import android.widget.ImageButton
import androidx.appcompat.app.AppCompatActivity
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.serialize
import com.mythicpalette.andropen.helpers.ConnectionState
import com.mythicpalette.andropen.helpers.Settings
import com.mythicpalette.andropen.helpers.SocketHandler
import com.mythicpalette.andropen.helpers.SocketStateListener
import com.mythicpalette.andropen.views.SignalButton
import com.mythicpalette.andropen.views.TouchInputView
import java.nio.ByteBuffer
import java.nio.ByteOrder

class MainActivity : AppCompatActivity() {
    private var socketHandler: SocketHandler = SocketHandler();

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        Settings.init(this)

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

        val drawArea =  findViewById<TouchInputView>(R.id.drawArea)
        //.socketHandler = this.socketHandler
        drawArea.onTouch = { infos ->
            // Create the byte buffer.
            val buffer =
                ByteBuffer.allocate(4 + (68 * infos.size)).order(ByteOrder.LITTLE_ENDIAN)

            // Serialize the number of touch events to send.
            buffer.putInt(infos.size)

            // Serialize the touch data.
            for (pi in infos)
                buffer.put(pi.serialize())

            // Send the data.
            socketHandler.send(buffer.array())
        }
        drawArea.onHover = { pi ->
            // Send the pointer count.
            val buffer = ByteBuffer.allocate(4 + 68).order(ByteOrder.LITTLE_ENDIAN)

            buffer.putInt(1)
            buffer.put(pi.serialize())

            socketHandler.send(buffer.array())
        }
        findViewById<ImageButton>(R.id.show_settings_button).setOnClickListener{ _ ->
            val intent = Intent(this, SettingsActivity::class.java)
            startActivity(intent)
            true
        }
    }
}