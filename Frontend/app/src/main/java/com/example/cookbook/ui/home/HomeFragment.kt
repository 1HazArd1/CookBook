package com.example.cookbook.ui.home

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageButton
import android.widget.SearchView
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import androidx.recyclerview.widget.GridLayoutManager
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.cookbook.R
import com.example.cookbook.services.adapters.CuisineAdapter
import com.example.cookbook.services.adapters.RecipeAdapter
import com.example.cookbook.services.apiInterfaces.HomeApiInterface
import com.example.cookbook.services.apiService.RetrofitService
import com.example.cookbook.services.sharedPreference.PreferenceManager
import com.example.cookbook.ui.createRecipe.CreateRecipeActivity
import com.example.cookbook.ui.user.UserActivity
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import retrofit2.HttpException
import java.io.IOException


private const val TAG = "HomeFragment"

data class SearchRecipeText(val searchText: String?)

class HomeFragment : Fragment() {

    private var itemView: View? = null
    private var btnLogout: Button? = null
    private var searchView: androidx.appcompat.widget.SearchView? = null
    private var noRecipeTextView: TextView? = null
    private var btnCreateRecipe : ImageButton? = null

    private var cuisineRecyclerView: RecyclerView? = null
    private var recipeRecyclerView: RecyclerView? = null

    private var isSearchCall: Int = 0  // 0 -> load up all recipe call 1 -> search recipe call
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        itemView = inflater.inflate(R.layout.home_fragment, container, false)

        btnLogout = itemView?.findViewById(R.id.btn_logout)
        searchView = itemView?.findViewById(R.id.sv_recipes)
        noRecipeTextView = itemView?.findViewById(R.id.tv_noRecipe)
        btnCreateRecipe = itemView?.findViewById(R.id.btn_createRecipe)

        cuisineRecyclerView = itemView?.findViewById(R.id.rv_cuisines)
        recipeRecyclerView = itemView?.findViewById(R.id.rv_recipes)


        btnLogout?.setOnClickListener {
            //clear jwt token and then navigate to login
            Log.d(TAG, "Logging Out")
            clearAuthToken()
            activity?.let {
                val intent = Intent(context, UserActivity::class.java)
                startActivity(intent)
            }
            activity?.finish()
        }

        //Get all cuisines
        getAllCuisines()

        //search based on recipe
        searchView?.setOnQueryTextListener(object : SearchView.OnQueryTextListener,
            androidx.appcompat.widget.SearchView.OnQueryTextListener {
            override fun onQueryTextSubmit(query: String?): Boolean {
                isSearchCall = 1
                getRecipes(SearchRecipeText(query), isSearchCall)
                return true
            }

            override fun onQueryTextChange(newText: String?): Boolean {
                getRecipes(SearchRecipeText(newText), isSearchCall)
                return true
            }

        })
        //Get all recipes current user + existing
        isSearchCall = 0
        getRecipes(SearchRecipeText(""), isSearchCall)

        Log.d(TAG, "Going to recipe fragment")
        btnCreateRecipe?.setOnClickListener{
            activity?.let {
                val intent = Intent(context, CreateRecipeActivity::class.java)
                startActivity(intent)
            }
        }


        return itemView
    }

    private fun getRetrofitClient(): HomeApiInterface {
        val retrofitService = RetrofitService(requireContext())
        return retrofitService.retrofit.create(HomeApiInterface::class.java)
    }

    private fun getAllCuisines() {
        lifecycleScope.launch {
            lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED) {
                val retrofitClient = getRetrofitClient()
                Log.d(TAG, "Calling Cuisine API")
                val response = try {
                    Log.d(TAG, "Trying to get cuisines")
                    retrofitClient.getAllCuisine()
                } catch (e: IOException) {
                    Log.e(TAG, "I/O Exception ${e.message}")
                    return@repeatOnLifecycle
                } catch (e: HttpException) {
                    Log.e(TAG, "Http Exception ${e.message}")
                    return@repeatOnLifecycle
                } catch (e: Exception) {
                    Log.e(TAG, "Exception ${e.message}")
                    return@repeatOnLifecycle
                }
                Log.d(TAG, response.body().toString())
                when {
                    response.isSuccessful && response.body() != null -> {
                        withContext(Dispatchers.Main) {
                            //successfully fetched api and received the response
                            Log.d(TAG, "Response Received")
                            val cuisines = response.body()!!
                            if (cuisines.isNotEmpty()) {
                                val cuisineAdapter = CuisineAdapter(cuisines)
                                val gridLayoutManager = GridLayoutManager(
                                    context,
                                    2,
                                    GridLayoutManager.HORIZONTAL,
                                    false
                                )
                                cuisineRecyclerView?.layoutManager = gridLayoutManager
                                cuisineRecyclerView?.adapter = cuisineAdapter
                            }
                        }
                    }

                    response.isSuccessful && response.body() == null -> {
                        Log.d(TAG, "Response Received with no data")
                    }

                    else -> {
                        val errorResponse = response.toString()
                        Log.e(TAG, "error : $errorResponse")
                    }
                }
            }
        }
    }

    private fun getRecipes(searchText: SearchRecipeText?, isSearchCall: Int) {
        lifecycleScope.launch {
            lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED) {
                val retrofitClient = getRetrofitClient()
                Log.d(TAG, "Calling Recipe API")
                val response = try {
                    Log.d(TAG, "Trying to get recipes")
                    retrofitClient.getAllRecipe(searchText)
                } catch (e: IOException) {
                    Log.e(TAG, "I/O Exception ${e.message}")
                    return@repeatOnLifecycle
                } catch (e: HttpException) {
                    Log.e(TAG, "Http Exception ${e.message}")
                    return@repeatOnLifecycle
                } catch (e: Exception) {
                    Log.e(TAG, "Exception ${e.message}")
                    return@repeatOnLifecycle
                }
                Log.d(TAG, response.body().toString())
                when {
                    response.isSuccessful && response.body() != null -> {
                        withContext(Dispatchers.Main) {
                            //successfully fetched api and received the response
                            Log.d(TAG, "Response Received")
                            val recipes = response.body()!!
                            if (recipes.isEmpty() && isSearchCall == 1) {
                                val emptyRecipeCallText = "No Recipes found for the search text"
                                noRecipeTextView?.visibility = View.VISIBLE
                                noRecipeTextView?.text = emptyRecipeCallText
                                recipeRecyclerView?.visibility = View.GONE
                            }
                            if (recipes.isNotEmpty()) {
                                //show all the recipes
                                noRecipeTextView?.visibility = View.GONE
                                recipeRecyclerView?.visibility = View.VISIBLE
                                val recipeAdapter = RecipeAdapter(recipes)
                                recipeRecyclerView?.layoutManager =
                                    LinearLayoutManager(requireContext())
                                recipeRecyclerView?.adapter = recipeAdapter
                            }
                            if (recipes.isEmpty() && isSearchCall == 0) {
                                // in case no recipes has been added  show the default text
                                noRecipeTextView?.visibility = View.VISIBLE
                                recipeRecyclerView?.visibility = View.GONE
                            }

                        }
                    }

                    response.isSuccessful && response.body() == null -> {
                        withContext(Dispatchers.Main) {
                            // successfully fetched api but received null response
                            Log.d(TAG, "Response Received with no data")
                            recipeRecyclerView?.visibility = View.GONE
                            noRecipeTextView?.visibility = View.VISIBLE
                        }
                    }

                    else -> {
                        // any other case like unauthorised response
                        val errorResponse = response.toString()
                        Log.e(TAG, "error : $errorResponse")
                    }

                }
            }
        }
    }

    private fun clearAuthToken() {
        context?.let {
            val preferenceManager = PreferenceManager(it)
            preferenceManager.clearToken()
        }
    }
}