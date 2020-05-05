package com.example.facematchdoorlock

import android.content.Intent
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

    lateinit var addUserButton: Button

    lateinit var deleteUserButton: Button

    lateinit var listUserButton: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        unlockButton = findViewById(R.id.unlockButton)
        checkButton = findViewById(R.id.checkButton)
        addUserButton = findViewById(R.id.addUserButton)
        deleteUserButton = findViewById(R.id.deleteUserButton)
        listUserButton = findViewById(R.id.listUserButton)
        val manager = APImanager()

        unlockButton.setOnClickListener { view: View ->
            val request = Request.Builder()
                .header("ApiKey", "MySecretKey")
                .url("http://192.168.86.41:5002/unlock")
                .build()
            doAsync {
                val response = manager.okHttpClient.newCall(request).execute()
                //val message = response.isSuccessful.toString()
                runOnUiThread {
                    Toast.makeText(
                        this@MainActivity,
                        "Unlocked.",
                        Toast.LENGTH_LONG
                    ).show()
                }
            }
        }

        checkButton.setOnClickListener { view: View ->
            val intent = Intent(this@MainActivity, CheckDoorActivity::class.java)
            startActivity(intent)
        }

        addUserButton.setOnClickListener { view: View ->
            val intent = Intent(this@MainActivity, AddUserActivity::class.java)
            startActivity(intent)
        }

        deleteUserButton.setOnClickListener { view: View ->
            val intent = Intent(this@MainActivity, DeleteUserActivity::class.java)
            startActivity(intent)
        }

        listUserButton.setOnClickListener { view: View ->
            val intent = Intent(this@MainActivity, ListUserActivity::class.java)
            startActivity(intent)
        }
    }
}
