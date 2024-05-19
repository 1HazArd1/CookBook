package com.example.cookbook.user

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.cookbook.R

class RegisterFragment : Fragment() {

    private var itemView : View? = null
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = layoutInflater.inflate(R.layout.register_fragment, container, false)

        return itemView
    }
}