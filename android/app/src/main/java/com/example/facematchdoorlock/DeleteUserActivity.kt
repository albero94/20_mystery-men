package com.example.facematchdoorlock

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.Spinner
import android.widget.Toast
import okhttp3.Request
import org.jetbrains.anko.doAsync
import org.json.JSONArray

class DeleteUserActivity : AppCompatActivity() {

    lateinit var selectUser: Spinner
    lateinit var deleteButton: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_delete_user)

        selectUser = findViewById(R.id.userSelect)
        selectUser.isEnabled = false
        deleteButton = findViewById(R.id.deleteButton)

        val manager = APImanager()

        deleteButton.setOnClickListener{
            val deleteRequest = Request.Builder()
                .url("https://gw-iot-facerecognitionserver.azurewebsites.net/facerecognition/DeletePerson?name=${selectUser.selectedItem}")
                .header("ApiKey", "MySecretKey")
                .delete()
                .build()
            doAsync {
                val response = manager.okHttpClient.newCall(deleteRequest).execute()
                runOnUiThread {
                    if (response.isSuccessful) {
                        Toast.makeText(
                            this@DeleteUserActivity,
                            "Deleted ${selectUser.selectedItem}",
                            Toast.LENGTH_LONG
                        ).show()
                        val intent = Intent(this@DeleteUserActivity, MainActivity::class.java)
                        startActivity(intent)
                    }
                }
            }
        }

        val request = Request.Builder()
            .url("https://gw-iot-facerecognitionserver.azurewebsites.net/facerecognition/ListPeople")
            .header("ApiKey", "MySecretKey")
            .build()
        val names: MutableList<String> = mutableListOf()

        doAsync {
            val response = manager.okHttpClient.newCall(request).execute()

            val responseString: String? = response.body?.string()

            if (!responseString.isNullOrEmpty() && response.isSuccessful) {
                val results = JSONArray(responseString)
                for (i in 0 until results.length())
                    names.add(results[i].toString())
            }
            runOnUiThread {
                selectUser.adapter = ArrayAdapter<String>(
                    this@DeleteUserActivity,
                    R.layout.support_simple_spinner_dropdown_item,
                    names
                )
                selectUser.isEnabled = true
                deleteButton.isEnabled = true
            }
        }

    }
}
