package com.example.facematchdoorlock

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.Button
import android.widget.Toast
import okhttp3.Request
import org.jetbrains.anko.doAsync

class MainActivity : AppCompatActivity() {

    lateinit var unlockButton: Button

    lateinit var checkButton: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        unlockButton = findViewById(R.id.unlockButton)
        checkButton = findViewById(R.id.checkButton)
        val manager = APImanager()

        unlockButton.setOnClickListener { view: View ->
            val request = Request.Builder()
                .url("http://192.168.86.41:5002/unlock")
                .build()
            doAsync {
                val response = manager.okHttpClient.newCall(request).execute()
                //val message = response.isSuccessful.toString()
                runOnUiThread{Toast.makeText(this@MainActivity, response.toString(), Toast.LENGTH_LONG).show()}
            }
        }

        checkButton.setOnClickListener { view: View ->
            val request = Request.Builder()
                .url("http://192.168.86.41:5002/checkdoor")
                .build()
            doAsync {
                val response = manager.okHttpClient.newCall(request).execute()
                //val message = response.isSuccessful.toString()
                runOnUiThread{Toast.makeText(this@MainActivity, response.toString(), Toast.LENGTH_LONG).show()}
            }
        }
    }
}
