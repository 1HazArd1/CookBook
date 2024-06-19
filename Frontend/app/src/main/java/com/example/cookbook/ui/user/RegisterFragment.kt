package com.example.cookbook.ui.user

import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import com.example.cookbook.R
import com.example.cookbook.models.User
import com.example.cookbook.services.apiInterfaces.UserApiInterface
import com.example.cookbook.services.apiService.RetrofitService
import com.google.android.material.textfield.TextInputEditText
import kotlinx.coroutines.launch
import retrofit2.HttpException
import java.io.IOException

const val  TAG = "RegisterFragment"
class RegisterFragment : Fragment() {

    private var itemView : View? = null
    private var firstName : TextInputEditText ? = null
    private var  lastName : TextInputEditText? = null
    private var email: TextInputEditText? = null
    private var password : TextInputEditText? = null
    private var btnRegister : Button? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = layoutInflater.inflate(R.layout.register_fragment, container, false)

        firstName = itemView?.findViewById(R.id.et_firstName)
        lastName = itemView?.findViewById(R.id.et_lastName)
        email = itemView?.findViewById(R.id.et_registerEmail)
        password = itemView?.findViewById(R.id.et_registerPassword)

        btnRegister = itemView?.findViewById(R.id.btn_register)
        btnRegister?.setOnClickListener{
            Toast.makeText(context,"Register clicked", Toast.LENGTH_SHORT).show()

            val user = User(
                firstName = firstName?.text.toString(),
                lastName =  lastName?.text.toString(),
                email = email?.text.toString(),
                password = password?.text.toString()
            )
            Log.d(TAG,"User: $user")

            val retrofitClient = RetrofitService.retrofit.create(UserApiInterface :: class.java)

            lifecycleScope.launch {
                lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED) {
                    Log.e(TAG,"REGISTERING")
                    // do your work here
                    val response = try {
                        retrofitClient.register(user)
                    }
                    catch (e: IOException) {
                        Log.e(TAG, "I/O Exception: ${e.message}", e)
                        return@repeatOnLifecycle
                    } catch (e: HttpException) {
                        Log.e(TAG, "Http Exception: ${e.message}", e)
                        return@repeatOnLifecycle
                    }
                    catch (e:Exception){
                        Log.e(TAG, "Exception : ${e.message}",e)
                        return@repeatOnLifecycle
                    }
                    Log.d(TAG, "Response received ${response.body()} ${response.isSuccessful}")
                    when {
                        response.isSuccessful && response.body() != null -> {
                            Log.d(TAG,"Registered successfully")
                            val registrationId = response.body()
                            // Handle successful registration with the received registrationId (Long)
                            Toast.makeText(context, "Registered Successfully with ID $registrationId", Toast.LENGTH_SHORT).show()
                        }
                        response.isSuccessful && response.body() == null -> {
                            Log.d(TAG,"Registered successfully but improper response")
                            // Handle successful response but with a null body
                            Toast.makeText(context, "Registered Successfully, but no ID received", Toast.LENGTH_SHORT).show()
                        }
                        else -> {
                            Log.e(TAG,"Registration failed ${response.errorBody()?.string()}")
                            // Handle unsuccessful response
                            val errorMessage = response.errorBody()?.string() ?: "Unknown error occurred"
                            Toast.makeText(context, "Registration Failed: $errorMessage", Toast.LENGTH_SHORT).show()
                        }
                    }
                }
            }
        }

        return itemView
    }
}