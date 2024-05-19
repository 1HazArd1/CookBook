package com.example.cookbook.user

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.LinearLayout
import androidx.fragment.app.Fragment
import com.example.cookbook.R

class LoginFragment: Fragment() {
    private var itemView: View? = null
    private var registerUser : LinearLayout? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = layoutInflater.inflate(R.layout.login_fragment, container,false)

        registerUser = itemView?.findViewById(R.id.tv_userSignUp)


        registerUser?.setOnClickListener {
            val userRegister = RegisterFragment()
            activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.fl_fragment_container,userRegister)
                ?.addToBackStack(null)
                ?.commit()
        }
        return itemView
    }
}