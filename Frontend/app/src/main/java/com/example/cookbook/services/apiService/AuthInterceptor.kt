package com.example.cookbook.services.apiService

import com.example.cookbook.services.sharedPreference.PreferenceManager
import okhttp3.Interceptor
import okhttp3.Response

class AuthInterceptor(private val preferenceManager: PreferenceManager) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val originalRequest = chain.request()

        val token = preferenceManager.getToken()
        if (token.isNullOrEmpty()) {
            return chain.proceed(originalRequest)
        }

        val modifiedRequest = originalRequest.newBuilder()
            .addHeader("Authorization", "Bearer $token")
            .build()

        return chain.proceed(modifiedRequest)
    }
}