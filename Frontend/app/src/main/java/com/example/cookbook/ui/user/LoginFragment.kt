package com.example.cookbook.ui.user

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.LinearLayout
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import com.example.cookbook.R
import com.example.cookbook.models.LoginUser
import com.example.cookbook.services.apiInterfaces.UserApiInterface
import com.example.cookbook.services.apiService.RetrofitService
import com.example.cookbook.services.sharedPreference.PreferenceManager
import com.example.cookbook.ui.home.HomeActivity
import com.google.android.material.textfield.TextInputEditText
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import retrofit2.HttpException
import java.io.IOException

private const val TAG = "LoginFragment"

class LoginFragment: Fragment() {
    private var itemView: View? = null
    private var registerUser : LinearLayout? = null
    private var loginEmail : TextInputEditText? = null
    private var loginPassword : TextInputEditText? = null
    private var btnLogin : Button? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = layoutInflater.inflate(R.layout.login_fragment, container,false)

        registerUser = itemView?.findViewById(R.id.tv_userSignUp)
        loginEmail = itemView?.findViewById(R.id.et_loginEmail)
        loginPassword = itemView?.findViewById(R.id.et_loginPassword)
        btnLogin = itemView?.findViewById(R.id.btn_login)

        registerUser?.setOnClickListener {
            val userRegister = RegisterFragment()
            activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.fl_loginFragment_container,userRegister)
                ?.addToBackStack(null)
                ?.commit()
        }

        btnLogin?.setOnClickListener {
            val user = LoginUser(
                email =  loginEmail?.text.toString().trim(),
                password = loginPassword?.text.toString().trim()
            )
            Log.d(TAG,"User: $user")

            lifecycleScope.launch {
                lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED){
                    val retrofitService = RetrofitService(requireContext())
                    val retrofitClient = retrofitService.retrofit.create(UserApiInterface ::class.java)
                    Log.d(TAG,"Login Initiated")
                    val response = try {
                        retrofitClient.login(user)
                    }
                    catch (e : IOException){
                        Log.e(TAG,"I/O Exception ${e.message}")
                        return@repeatOnLifecycle
                    }
                    catch (e : HttpException){
                        Log.e(TAG, "Http Exception ${e.message}")
                        return@repeatOnLifecycle
                    }
                    catch (e : Exception){
                        Log.e(TAG, "Exception ${e.message}")
                        return@repeatOnLifecycle
                    }
                    when{
                        response.isSuccessful && response.body() != null -> {
                            withContext(Dispatchers.Main) {
                                //login successful and auth token received
                                val authToken = response.body()?.accessToken
                                Log.d(TAG,"JWT Token: $authToken")
                                if(!authToken.isNullOrEmpty())
                                    saveAuthToken(authToken)

                                else {
                                    Log.e(TAG, "Token not found in response")
                                    return@withContext
                                }
                                Log.d(TAG,"Login successful")
                                Toast.makeText(context, "Login Successful", Toast.LENGTH_SHORT)
                                    .show()
                                Log.d(TAG,"Navigating to home page")
                                activity?.let {
                                    val intent = Intent(context, HomeActivity::class.java)
                                    startActivity(intent)
                                }
                                activity?.finish()

                            }
                        }
                        response.isSuccessful && response.body() == null -> {
                            //correct login credentials but did not receive any auth token in the response
                            Log.d(TAG, "Correct Login Credentials but failed to receive any response")
                            Toast.makeText(context,"Please retry", Toast.LENGTH_SHORT).show()
                        }
                        else ->{
                            //incorrect login credentials failed to login
                            withContext(Dispatchers.Main){
                                val errorResponse = response.errorBody()?.string()
                                Log.e(TAG,"error : $errorResponse")
                                Toast.makeText(context, "Incorrect credentials", Toast.LENGTH_SHORT).show()
                            }
                        }
                    }
                }
            }
        }

        return itemView
    }
    private fun saveAuthToken(authToken: String){
        context?.let {
            val preferenceManager= PreferenceManager(it)
            preferenceManager.saveToken(authToken)
        }
    }
}