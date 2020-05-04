// Credit for assistance here:
// https://medium.com/better-programming/how-to-upload-an-image-file-to-your-server-using-volley-in-kotlin-a-step-by-step-tutorial-23f3c0603ec2

package com.example.facematchdoorlock

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.ImageView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import okhttp3.MultipartBody
import okhttp3.Request
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody.Companion.toRequestBody
import org.jetbrains.anko.doAsync


class AddUserActivity : AppCompatActivity() {

    lateinit var name: EditText
    lateinit var imageView: ImageView
    lateinit var selectImage: Button
    lateinit var goButton: Button
    var image: ByteArray? = null
    var uri: Uri? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_add_user)

        name = findViewById(R.id.name)
        imageView = findViewById(R.id.imageView)
        selectImage = findViewById(R.id.selectImageButton)
        goButton = findViewById(R.id.goButton)

        selectImage.setOnClickListener{
            val intent = Intent(Intent.ACTION_PICK)
            intent.type = "image/*"
            startActivityForResult(intent, 999)
        }

        goButton.setOnClickListener{
            val manager = APImanager()
            val requestBody = MultipartBody.Builder()
                .setType(MultipartBody.FORM)
                .addFormDataPart("name", name.text.toString())
                .addFormDataPart("FormFiles", "image", body = image!!.toRequestBody("image/jpg".toMediaType()))
                .build()
            val request = Request.Builder()
                .header("ApiKey", "MySecretKey")
                .url("https://gw-iot-facerecognitionserver.azurewebsites.net/facerecognition/CreatePerson")
                .post(requestBody)
                .build()
            doAsync {
                val response = manager.okHttpClient.newCall(request).execute()
                runOnUiThread {
                    if (response.isSuccessful) {
                        Toast.makeText(
                            this@AddUserActivity,
                            "Added User",
                            Toast.LENGTH_LONG
                        ).show()
                        val intent = Intent(this@AddUserActivity, MainActivity::class.java)
                        startActivity(intent)
                    }
                }
            }

        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        if (resultCode == Activity.RESULT_OK && requestCode == 999) {
            uri = data?.data
            if (uri != null) {
                imageView.setImageURI(uri)
                createImageData(uri!!)
            }
        }
        super.onActivityResult(requestCode, resultCode, data)
    }

    private fun createImageData(uri: Uri) {
        val inputStream = contentResolver.openInputStream(uri)
        inputStream?.buffered()?.use {
            image = it.readBytes()
        }
    }
}
