package com.example.cookbook.services.sharedPreference

import android.content.Context
import android.content.SharedPreferences
import com.example.cookbook.R

class PreferenceManager(context: Context) {

    private val preferences: SharedPreferences = context.getSharedPreferences(R.string.preference_file_key.toString(), Context.MODE_PRIVATE)

    fun saveToken(token: String) {
        val editor = preferences.edit()
        editor.putString("jwt_token", token)
        editor.apply()
    }

    fun getToken(): String? {
        return preferences.getString("jwt_token", null)
    }

    fun clearToken() {
        val editor = preferences.edit()
        editor.remove("jwt_token")
        editor.apply()
    }
}