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
import com.mythicpalette.andropen.helpers.SettingsEventListener
import com.mythicpalette.andropen.helpers.SocketHandler
import com.mythicpalette.andropen.helpers.SocketStateListener
import com.mythicpalette.andropen.views.SignalButton
import com.mythicpalette.andropen.views.TouchInputView
import com.mythicpalette.andropen.views.TouchSliderView
import com.mythicpalette.andropen.views.TouchToggleButton
import java.nio.ByteBuffer
import java.nio.ByteOrder

class MainActivity : AppCompatActivity() {
    private var socketHandler: SocketHandler = SocketHandler();

    companion object {
        const val DRAW_AREA_ID = 0
        const val SLIDER_1_ID = 1
        const val SLIDER_2_ID = 2
    }

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        Settings.init(this)

        socketHandler.addListener(object: SocketStateListener {
            override fun onConnectionStateChanged(state: ConnectionState) {
                findViewById<SignalButton>(R.id.reconnect_button).buttonOn = state.Connected
                if ( state.Connected ) println("Connected")
                else println("Disconnected")
            }
        })
        socketHandler.connectToSocket(this);
        findViewById<View>(R.id.reconnect_button).setOnClickListener{ v ->
            socketHandler.connectToSocket(this);
        }

        val touchDisable = findViewById<TouchToggleButton>(R.id.touch_disable_button)
        touchDisable.buttonOn = !Settings.TouchDisabled
        touchDisable.setOnClickListener{_ ->
            Settings.setTouchDisabled(this, !Settings.TouchDisabled)
        }
        val drawArea =  findViewById<TouchInputView>(R.id.drawArea)

        drawArea.onTouch = { id, infos ->
            onMultiTouch(id, infos)
        }

        drawArea.onHover = { id, infos ->
            onSingleTouch(id, infos)
        }

        val slider1 = findViewById<TouchSliderView>(R.id.dragSlider1)
        slider1.sensitivity = Settings.Slider1Sensitivity
        slider1.onTouch = { id, info ->
            onSingleTouch(id, info)
        }

        val slider2 = findViewById<TouchSliderView>(R.id.dragSlider2)
        slider2.sensitivity = Settings.Slider2Sensitivity
        slider2.onTouch = { id, info ->
            onSingleTouch(id, info)
        }

        val settingsListener = object : SettingsEventListener {
            override fun onSettingsChanged() {
                slider1.sensitivity = Settings.Slider1Sensitivity
                slider2.sensitivity = Settings.Slider2Sensitivity
                touchDisable.buttonOn = !Settings.TouchDisabled
            }
        }

        Settings.addListener(settingsListener)

        findViewById<ImageButton>(R.id.show_settings_button).setOnClickListener{ _ ->
            val intent = Intent(this, SettingsActivity::class.java)
            startActivity(intent)
            true
        }
    }

    fun onMultiTouch(id: Int, infos: MutableList<PointerInfo> ) {
        val buffer =
            ByteBuffer.allocate(8 + (56 * infos.size)).order(ByteOrder.LITTLE_ENDIAN)

        buffer.putInt(id)

        // Serialize the number of touch events to send.
        buffer.putInt(infos.size)

        // Serialize the touch data.
        for (pi in infos)
            buffer.put(pi.serialize())

        // Send the data.
        socketHandler.send(buffer.array())
    }

    fun onSingleTouch(id: Int, pi: PointerInfo ) {
        // Send the pointer count.
        val buffer = ByteBuffer.allocate(4 + 68).order(ByteOrder.LITTLE_ENDIAN)

        buffer.putInt(id) // This is the id of the sender.
        buffer.putInt(1) // There is only one pointer that can hover.
        buffer.put(pi.serialize()) // This is the PointerInfo serialized.

        socketHandler.send(buffer.array())
    }
}