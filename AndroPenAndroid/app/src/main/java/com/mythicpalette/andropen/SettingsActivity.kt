package com.mythicpalette.andropen

import android.os.Bundle
import android.widget.EditText
import android.widget.SeekBar
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.AppCompatButton
import androidx.appcompat.widget.SwitchCompat
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.mythicpalette.andropen.helpers.Settings

class SettingsActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_settings)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        findViewById<EditText>(R.id.ip_text).setText(Settings.IpAddress)
        findViewById<EditText>(R.id.port_text).setText(Settings.Port.toString())
        findViewById<SwitchCompat>(R.id.touch_switch).isChecked = Settings.PenBlocksTouch

        val s1 = findViewById<SeekBar>(R.id.sensitivity1)
        val s1Sense = (s1.max - ((Settings.Slider1Sensitivity * 100) - s1.min))

        val s2 = findViewById<SeekBar>(R.id.sensitivity2)
        val s2Sense = (s2.max - ((Settings.Slider2Sensitivity * 100) - s2.min))
        s1.progress = s1Sense.toInt()
        s2.progress = s2Sense.toInt()

        findViewById<AppCompatButton>(R.id.settings_save_button).setOnClickListener{ _ ->
            onSave()
            onBackPressedDispatcher.onBackPressed()
        }
        findViewById<AppCompatButton>(R.id.settings_cancel_button).setOnClickListener{ _ ->
            onBackPressedDispatcher.onBackPressed()
        }
    }

    private fun onSave() {
        val ip = findViewById<EditText>(R.id.ip_text).text.toString()
        val port = findViewById<EditText>(R.id.port_text).text.toString().toInt()
        val penTouch = findViewById<SwitchCompat>(R.id.touch_switch).isChecked

        val s1 = findViewById<SeekBar>(R.id.sensitivity1)
        val s1Sense = (s1.max - s1.progress + s1.min) / 100f

        val s2 = findViewById<SeekBar>(R.id.sensitivity2)
        val s2Sense = (s2.max - s2.progress + s2.min) / 100f

        Settings.set(
            this,
            ip=ip,
            port=port,
            penTouch=penTouch,
            slider1Sensitivity=s1Sense,
            slider2Sensitivity=s2Sense
        )
    }
}