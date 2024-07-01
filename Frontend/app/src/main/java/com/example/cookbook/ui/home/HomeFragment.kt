package com.example.cookbook.ui.home

import android.content.Intent
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import com.example.cookbook.R
import com.example.cookbook.services.sharedPreference.PreferenceManager
import com.example.cookbook.ui.user.UserActivity

private const val TAG = "HomeFragment"
class HomeFragment : Fragment() {

    private var itemView: View? = null
    private var btnLogout : Button? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = inflater.inflate(R.layout.home_fragment, container, false)

        btnLogout = itemView?.findViewById(R.id.btn_logout)

        btnLogout?.setOnClickListener {
            //clear jwt token and then navigate to login
            Log.d(TAG,"Logging Out")
            clearAuthToken()
            activity?.let {
                val intent = Intent(context, UserActivity ::class.java)
                startActivity(intent)
            }
            activity?.finish()
        }

        return itemView
    }

    private fun clearAuthToken(){
        context?.let {
            val preferenceManager = PreferenceManager(it)
            preferenceManager.clearToken()
        }
    }
}