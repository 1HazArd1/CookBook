package com.example.cookbook.ui.home

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.cookbook.R
import com.example.cookbook.models.Cuisine
import com.example.cookbook.services.adapters.CuisineAdapter
import com.example.cookbook.services.adapters.RecipeAdapter
import com.example.cookbook.services.apiInterfaces.HomeApiInterface
import com.example.cookbook.services.apiService.RetrofitService
import com.example.cookbook.services.sharedPreference.PreferenceManager
import com.example.cookbook.ui.user.UserActivity
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import retrofit2.HttpException
import java.io.IOException


private const val TAG = "HomeFragment"
class HomeFragment : Fragment() {

    private var itemView: View? = null
    private var btnLogout : Button? = null
    private var cuisineRecyclerView : RecyclerView? = null
    private var recipeRecyclerView : RecyclerView? = null
    private var noRecipeTextView : TextView? = null

    private lateinit var  cuisineList : ArrayList<Cuisine>
    private lateinit var cuisineAdapter: CuisineAdapter

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = inflater.inflate(R.layout.home_fragment, container, false)

        btnLogout = itemView?.findViewById(R.id.btn_logout)
        cuisineRecyclerView = itemView?.findViewById(R.id.rv_cuisines)
        recipeRecyclerView = itemView?.findViewById(R.id.rv_recipes)
        noRecipeTextView = itemView?.findViewById(R.id.tv_noRecipe)

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

        //Get all cuisines
        getAllCuisines()
        //Get all recipes current user + existing
        getAllRecipes()


        return itemView
    }
    private fun getRetrofitClient(): HomeApiInterface{
        val retrofitService = RetrofitService(requireContext())
        return retrofitService.retrofit.create(HomeApiInterface ::class.java)
    }

    private fun getAllCuisines(){
        lifecycleScope.launch {
            lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED){
                val retrofitClient  = getRetrofitClient()
                Log.d(TAG, "Calling Cuisine API")
                val response = try{
                    Log.d(TAG,"Trying to get cuisines")
                    retrofitClient.getAllCuisine()
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
                    Log.e(TAG,"Exception ${e.message}")
                    return@repeatOnLifecycle
                }
                Log.d(TAG, response.body().toString())
                when{
                    response.isSuccessful && response.body() != null -> {
                        withContext(Dispatchers.Main) {
                            //successfully fetched api and received the response
                            Log.d(TAG, "Response Received")
                            val cuisines = response.body()!!
                            if(cuisines.isNotEmpty()){
                                val cuisineAdapter = CuisineAdapter(cuisines)
                                val gridLayoutManager = GridLayoutManager(context, 2, GridLayoutManager.HORIZONTAL,false)
                                cuisineRecyclerView?.layoutManager = gridLayoutManager
                                cuisineRecyclerView?.adapter = cuisineAdapter
                            }
                        }
                    }
                }
            }
        }
    }

    private fun getAllRecipes(){
        lifecycleScope.launch {
            lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED){
                val retrofitClient = getRetrofitClient()
                Log.d(TAG, "Calling Recipe API")
                val response = try{
                    Log.d(TAG,"Trying to get recipes")
                    retrofitClient.getAllRecipe()
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
                    Log.e(TAG,"Exception ${e.message}")
                    return@repeatOnLifecycle
                }
                Log.d(TAG, response.body().toString())
                when{
                    response.isSuccessful && response.body() != null -> {
                        withContext(Dispatchers.Main) {
                            //successfully fetched api and received the response
                            Log.d(TAG, "Response Received")
                            val recipes = response.body()!!
                            if(recipes.isNotEmpty()){
                                //show all the recipes
                                noRecipeTextView?.visibility = View.GONE
                                recipeRecyclerView?.visibility = View.VISIBLE
                                val recipeAdapter = RecipeAdapter(recipes)
                                recipeRecyclerView?.layoutManager = LinearLayoutManager(requireContext())
                                recipeRecyclerView?.adapter = recipeAdapter
                            }
                            else{
                                // in case no recipes hase been added  show the default text
                                noRecipeTextView?.visibility = View.VISIBLE
                                recipeRecyclerView?.visibility = View.GONE
                            }

                        }
                    }
                    response.isSuccessful && response.body() == null -> {
                        withContext(Dispatchers.Main){
                            // successfully fetched api but received null response
                            Log.d(TAG, "Response Received with no data")
                            recipeRecyclerView?.visibility = View.GONE
                            noRecipeTextView?.visibility = View.VISIBLE
                        }
                    }
                    else ->{
                        // any other case like unauthorised response
                        val errorResponse = response.toString()
                        Log.e(TAG,"error : $errorResponse")
                    }

                }
            }
        }
    }

    private fun clearAuthToken(){
        context?.let {
            val preferenceManager = PreferenceManager(it)
            preferenceManager.clearToken()
        }
    }
}