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

                    if (verifyUserData()) {
                        val retrofitClient =
                            RetrofitService.retrofit.create(UserApiInterface::class.java)
                        Log.d(TAG, "REGISTERING")
                        // call the register api
                        val response = try {
                            retrofitClient.register(user)
                        } catch (e: IOException) {
                            Log.e(TAG, "I/O Exception: ${e.message}", e)
                            return@repeatOnLifecycle
                        } catch (e: HttpException) {
                            Log.e(TAG, "Http Exception: ${e.message}", e)
                            return@repeatOnLifecycle
                        } catch (e: Exception) {
                            Log.e(TAG, "Exception : ${e.message}", e)
                            return@repeatOnLifecycle
                        }
                        Log.d(TAG, "Response received ${response.body()}")

                        when {
                            response.isSuccessful && response.body() != null -> {
                                Log.d(TAG, "Registered successfully")
                                withContext(Dispatchers.Main) {
                                    val registrationId = response.body()
                                    Log.d(TAG,"UserId : $registrationId")
                                    // Successful Registration
                                    Toast.makeText(
                                        context,
                                        "Registered Successfully",
                                        Toast.LENGTH_SHORT
                                    ).show()
                                    activity?.let {
                                        val intent = Intent(context, HomeActivity::class.java)
                                        startActivity(intent)
                                    }
                                    activity?.finish()
                                }
                            }

                            response.isSuccessful && response.body() == null -> {
                                Log.d(TAG, "Registered successfully but improper response")
                                // Successful registration failed to get response
                                withContext(Dispatchers.Main) {
                                    Toast.makeText(
                                        context,
                                        "Registered Successfully, but no ID received",
                                        Toast.LENGTH_SHORT
                                    ).show()
                                }
                            }

                            else -> {
                                Log.e(TAG, "Registration failed ${response.errorBody()?.string()}")
                                // Handle unsuccessful response
                                withContext(Dispatchers.Main) {
                                    Toast.makeText(
                                        context,
                                        "Something went wrong registration failed",
                                        Toast.LENGTH_SHORT
                                    ).show()
                                }
                            }
                        }
                    }
                }
            }
        }

        return itemView
    }
}