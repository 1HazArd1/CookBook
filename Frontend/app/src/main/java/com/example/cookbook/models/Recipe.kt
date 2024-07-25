package com.example.cookbook.models

data class Recipe(
    val id : Long,
    val name : String,
    val cuisine : String,
    val recipeUrl : String,
    val duration : Int,
    val servings : Byte,
    val isEditable : Boolean
)