package com.example.cookbook.ui.createRecipe

import com.example.cookbook.models.Recipe

interface RecipeCommunicator {
    fun passRecipeData(recipe : Recipe)
    fun getRecipeData() : Recipe
    fun passIngredientsData (ingredients : String)
    fun getIngredients() : String
}