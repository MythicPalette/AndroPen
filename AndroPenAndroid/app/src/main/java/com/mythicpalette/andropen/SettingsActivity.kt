package com.mythicpalette.andropen

import android.os.Bundle
import android.widget.EditText
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

        findViewById<EditText>(R.id.ip_text).setText(Settings.getIpAddress(this))
        findViewById<EditText>(R.id.port_text).setText(Settings.getPort(this).toString())
        findViewById<SwitchCompat>(R.id.touch_switch).isChecked = Settings.getPenBlocksTouch(this)

        findViewById<AppCompatButton>(R.id.settings_save_button).setOnClickListener{ _ ->
            onSave()
            onBackPressedDispatcher.onBackPressed()
        }
        findViewById<AppCompatButton>(R.id.settings_cancel_button).setOnClickListener{ _ ->
            onBackPressedDispatcher.onBackPressed()
        }
    }

    fun onSave() {
        // Get SharedPreferences instance
        val sharedPreferences = getSharedPreferences("MyAppPreferences", MODE_PRIVATE)
        val editor = sharedPreferences.edit()

        // push the values.
        // IP
        val ipText = findViewById<EditText>(R.id.ip_text).text.toString()
        editor.putString("ip_address", ipText)
        // Port
        val portText = findViewById<EditText>(R.id.port_text).text.toString().toInt()
        editor.putInt("port_address", portText)
        // Block Touch when pen is down
        val blockTouch = findViewById<SwitchCompat>(R.id.touch_switch).isChecked
        editor.putBoolean("block_touch_pen", blockTouch)  // Save asynchronously

        editor.apply()
    }
}