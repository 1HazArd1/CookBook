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

        val gridLayoutManager = GridLayoutManager(context, 2, GridLayoutManager.HORIZONTAL,false)
        cuisineRecyclerView?.layoutManager = gridLayoutManager

        cuisineList = ArrayList()
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/chinese.png?alt=media&token=3776c5d9-4df1-41a4-92a4-0bea1a622cc8","Chinese"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/indian.png?alt=media&token=554408a2-88a7-4f2e-9754-8fcaa1256f24","Indian"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/italian.png?alt=media&token=f7493366-06fb-4681-885c-e3664734734f","Italian"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/french.png?alt=media&token=8f678538-bba5-423e-a0d5-95a464a07789","French"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/japanese.png?alt=media&token=8e769e51-b541-48e3-b2f5-a2ac304045ca","Japanese"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/korean.png?alt=media&token=efa97da0-5527-4fb0-89e1-33531366aff8","Korean"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/malaysian.png?alt=media&token=aee2d45d-fc77-4ef7-b739-ea7fa8bf8028","Malaysian"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/mexican.png?alt=media&token=88a83b46-1447-4e69-a86a-4915248da034","Mexican"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/nepali.png?alt=media&token=e1a239ea-36de-4758-a2ed-0cf2b52bcc37","Nepali"))
        cuisineList.add(Cuisine("https://firebasestorage.googleapis.com/v0/b/demoproject-d7987.appspot.com/o/thailand.png?alt=media&token=b9750c39-4565-47ae-b6f8-851613379b8c","Thai"))

        cuisineAdapter = CuisineAdapter(cuisineList)
        cuisineRecyclerView?.adapter = cuisineAdapter

        //Get all recipes current user + existing
        getAllRecipes()


        return itemView
    }
    private fun getRetrofitClient(): HomeApiInterface{
        val retrofitService = RetrofitService(requireContext())
        return retrofitService.retrofit.create(HomeApiInterface ::class.java)
    }

    private fun getAllRecipes(){
        lifecycleScope.launch {
            lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED){
                val retrofitClient = getRetrofitClient()
                Log.d(TAG, "Calling Recipe API")
                val response = try{
                    Log.d(TAG,"Trying to get response")
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
                            val list = response.body()!!
                            if(list.isNotEmpty()){
                                //show all the recipes
                                noRecipeTextView?.visibility = View.GONE
                                recipeRecyclerView?.visibility = View.VISIBLE
                                val recipeAdapter = RecipeAdapter(response.body())
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