package com.example.cookbook.services.apiInterfaces

import com.example.cookbook.models.Recipe
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface RecipeApiInterface {

    @POST("Recipe")
    suspend fun createUserRecipe(@Body userRecipe : Recipe) : Response<Long>
}