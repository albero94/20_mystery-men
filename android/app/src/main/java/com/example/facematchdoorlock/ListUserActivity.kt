package com.example.facematchdoorlock

import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import okhttp3.Request
import org.jetbrains.anko.doAsync
import org.json.JSONArray
import java.util.*

class ListUserActivity : AppCompatActivity() {

    lateinit var recyclerView: RecyclerView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_list_user)

        recyclerView = findViewById(R.id.recyclerView)
        recyclerView.layoutManager = LinearLayoutManager(this)

        val manager = APImanager()
        val people: MutableList<Person> = mutableListOf()
        val request = Request.Builder()
            .header("ApiKey", "MySecretKey")
            .url("https://gw-iot-facerecognitionserver.azurewebsites.net/facerecognition/ListPeopleWithImages")
            .get()
            .build()
        doAsync {
            val response = manager.okHttpClient.newCall(request).execute()
            val responseString: String? = response.body?.string()

            if (!responseString.isNullOrEmpty() && response.isSuccessful) {
                val results = JSONArray(responseString)
                for (i in 0 until results.length()){
                    val curr = results.getJSONObject(i)
                    val name = curr.getString("name")
                    val img = Base64.getDecoder().decode(curr.getString("image"))

                    val person = Person(
                        name = name,
                        img = BitmapFactory.decodeByteArray(img, 0, img.size)
                    )
                    people.add(person)
                }
            }
            runOnUiThread {
                val adapter = ListAdapter(people)
                recyclerView.adapter = adapter
            }
        }
    }
}
