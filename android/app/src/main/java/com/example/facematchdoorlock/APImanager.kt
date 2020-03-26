package com.example.facematchdoorlock

import java.util.concurrent.TimeUnit
import okhttp3.OkHttpClient

class APImanager{
    public val okHttpClient: OkHttpClient

    init {
        val builder = OkHttpClient.Builder()

        builder.connectTimeout(15, TimeUnit.SECONDS)
        builder.readTimeout(15, TimeUnit.SECONDS)
        builder.writeTimeout(15, TimeUnit.SECONDS)

        okHttpClient = builder.build()
    }
}