package com.example.cookbook.ui.user

import android.content.Intent
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
import com.example.cookbook.ui.home.HomeActivity
import com.google.android.material.textfield.TextInputEditText
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import retrofit2.HttpException
import java.io.IOException

private const val TAG = "RegisterFragment"

class RegisterFragment : Fragment() {

    private var itemView: View? = null
    private var firstName: TextInputEditText? = null
    private var lastName: TextInputEditText? = null
    private var registerEmail: TextInputEditText? = null
    private var registerPassword: TextInputEditText? = null
    private var btnRegister: Button? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = inflater.inflate(R.layout.register_fragment, container, false)

        firstName = itemView?.findViewById(R.id.et_firstName)
        lastName = itemView?.findViewById(R.id.et_lastName)
        registerEmail = itemView?.findViewById(R.id.et_registerEmail)
        registerPassword = itemView?.findViewById(R.id.et_registerPassword)
        btnRegister = itemView?.findViewById(R.id.btn_register)

        btnRegister?.setOnClickListener {

            val user = User(
                firstName = firstName?.text.toString().trim(),
                lastName = if (lastName?.text.toString()
                        .isEmpty()
                ) null else lastName?.text.toString().trim(),
                email = registerEmail?.text.toString().trim(),
                password = registerPassword?.text.toString().trim()
            )
            Log.d(TAG, "User: $user")

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
                        Log.d(TAG, "Response received ${response.body()} ${response.isSuccessful}")

                        when {
                            response.isSuccessful && response.body() != null -> {
                                Log.d(TAG, "Registered successfully")
                                withContext(Dispatchers.Main) {
                                    val registrationId = response.body()
                                    // Successful Registration
                                    Toast.makeText(
                                        context,
                                        "Registered Successfully with ID $registrationId",
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

    private fun verifyUserData(): Boolean {

        if (firstName?.text.toString().isBlank()) {
            firstName?.error = "Required Field"
            return false
        }
        if (registerEmail?.text.toString().isBlank()) {
            registerEmail?.error = "Required Field"
            return false
        }
        if (registerPassword?.text.toString().isBlank()) {
            registerPassword?.error = "Password is Required"
            return false
        }
        if (!verifyEmail()) {
            registerEmail?.error = "Invalid Email"
            return false
        }
        if (!isValidPassword()) {
            registerPassword?.error = "Improper password format"
            return false
        }

        return true
    }

    private fun verifyEmail(): Boolean {
        return android.util.Patterns.EMAIL_ADDRESS.matcher(registerEmail?.text.toString()).matches()
    }

    private fun isValidPassword(): Boolean {
        val passwordString = registerPassword?.text.toString()
        return passwordString.length >= 8 &&
                passwordString.any { it.isDigit() } &&
                passwordString.any { it.isUpperCase() } &&
                passwordString.any { it.isLowerCase() } &&
                passwordString.any { !it.isLetterOrDigit() }
    }
}