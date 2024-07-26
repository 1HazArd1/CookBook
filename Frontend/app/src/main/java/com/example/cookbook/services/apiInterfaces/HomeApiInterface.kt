package com.example.cookbook.services.apiInterfaces

import com.example.cookbook.models.Cuisine
import com.example.cookbook.models.Recipe
import com.example.cookbook.models.RecipeInstruction
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path

interface HomeApiInterface {

    @GET("Home/cuisine")
    suspend fun getAllCuisine() : Response<List<Cuisine>>

    @GET("Home/recipe")
    suspend fun getAllRecipe() : Response<List<Recipe>>

    @GET("Home/recipe/{id}/ingredients")
    suspend fun getRecipeIngredients(@Path("id") id : Long) : Response<List<String>>

    @GET("Home/recipe/{id}/instruction")
    suspend fun getRecipeInstructions(@Path("id") id : Long) : Response<List<RecipeInstruction>>

}