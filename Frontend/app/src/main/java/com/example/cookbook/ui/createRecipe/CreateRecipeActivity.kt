package com.example.cookbook.ui.createRecipe

import android.os.Bundle
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.fragment.app.Fragment
import com.example.cookbook.R
import com.example.cookbook.models.Recipe

class CreateRecipeActivity : AppCompatActivity(), RecipeCommunicator {
    private lateinit var recipe: Recipe
    private  lateinit var ingredients : String
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_create_recipe)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.ll_createRecipeMain)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        addCreateRecipeFragment()
    }

    private fun addCreateRecipeFragment(){
        supportFragmentManager.beginTransaction().add(R.id.fl_createRecipe_container, CreateRecipeFragment())
            .commit()
    }

    override fun passRecipeData(recipe: Recipe) {
        this.recipe = recipe
    }

    override fun passIngredientsData(ingredients: String) {
        this.ingredients = ingredients
    }

    override fun getRecipeData(): Recipe = recipe

    override fun getIngredients(): String = ingredients

    fun replaceFragment(fragment : Fragment){
        val transaction = supportFragmentManager.beginTransaction()
        transaction.replace(R.id.fl_createRecipe_container, fragment)
        transaction.addToBackStack(null) // Add transaction to the back stack
        transaction.commit()
    }
}