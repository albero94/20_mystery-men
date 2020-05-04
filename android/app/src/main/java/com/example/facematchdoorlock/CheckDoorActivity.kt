package com.example.facematchdoorlock

import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ImageView
import okhttp3.Request
import org.jetbrains.anko.doAsync

class CheckDoorActivity : AppCompatActivity() {

    lateinit var img: ImageView
    var active: Boolean = true

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_check_door)

        img = findViewById(R.id.doorImage)
        val manager = APImanager()


        val request = Request.Builder()
            .header("ApiKey", "MySecretKey")
            .url("http://192.168.86.41:5002/checkdoor")
            .build()
        doAsync {
            while (active) {
                val response = manager.okHttpClient.newCall(request).execute()
                val bitmap = BitmapFactory.decodeStream(response.body?.byteStream())
                runOnUiThread { img.setImageBitmap(bitmap) }
                Thread.sleep(500)
            }
        }
    }

    override fun onBackPressed() {
        super.onBackPressed()
        active = false
    }

}
