package com.example.cookbook.services.apiInterfaces

import com.example.cookbook.models.Cuisine
import com.example.cookbook.models.Recipe
import com.example.cookbook.models.RecipeInstruction
import com.example.cookbook.ui.home.SearchRecipeText
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface HomeApiInterface {

    @GET("Home/cuisine")
    suspend fun getAllCuisine() : Response<List<Cuisine>>

    @POST("Home/recipe")
    suspend fun getAllRecipe(@Body searchRequest: SearchRecipeText?) : Response<List<Recipe>>

    @GET("Home/recipe/{id}/ingredients")
    suspend fun getRecipeIngredients(@Path("id") id : Long) : Response<List<String>>

    @GET("Home/recipe/{id}/instruction")
    suspend fun getRecipeInstructions(@Path("id") id : Long) : Response<List<RecipeInstruction>>

}