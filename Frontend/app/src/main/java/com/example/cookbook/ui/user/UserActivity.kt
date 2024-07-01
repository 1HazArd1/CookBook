package com.example.cookbook.ui.user

import android.content.Intent
import android.os.Bundle
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.example.cookbook.R
import com.example.cookbook.services.sharedPreference.PreferenceManager
import com.example.cookbook.ui.home.HomeActivity

class UserActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_user)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        val preferenceManager = PreferenceManager(this)
        if(!preferenceManager.getToken().isNullOrEmpty()){
            val intent =  Intent(this, HomeActivity :: class.java)
            startActivity(intent)
            finish()
        }
        addLoginFragment()
    }
    private fun addLoginFragment(){
        val userSignInLayout = LoginFragment()
        supportFragmentManager.beginTransaction().add(R.id.fl_loginFragment_container, userSignInLayout)
            .commit()
    }
}