package com.example.cookbook.ui.createRecipe

import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import androidx.fragment.app.Fragment
import com.example.cookbook.R
import com.google.android.material.textfield.TextInputEditText

private const val TAG = "CreateIngredientsFragment"
class CreateIngredientsFragment : Fragment() {

    private var itemView: View? = null

    private var etIngredients: TextInputEditText? = null
    private var btnIngredientNext: ImageButton? = null
    private lateinit var communicator: RecipeCommunicator

    override fun onAttach(context: Context) {
        super.onAttach(context)
        if (context is RecipeCommunicator) {
            communicator = context
        } else {
            throw RuntimeException("$context must implement RecipeCommunicator")
        }
    }
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {

        itemView = inflater.inflate(R.layout.create_ingredients_fragment, container, false)

        etIngredients = itemView?.findViewById(R.id.et_ingredients)
        btnIngredientNext = itemView?.findViewById(R.id.btn_ingredient_next)

        btnIngredientNext?.setOnClickListener {
            val ingredients = etIngredients?.text.toString()
            if(verifyIngredientsData(ingredients)) {
                val formattedIngredients = ingredients.replace("\n", "$")
                Log.d(TAG,"Formatted ingredients $formattedIngredients")
                //pass the ingredients to the instruction fragment to submit
                communicator.passIngredientsData(formattedIngredients)
                //replace current fragment with instruction fragment
                (activity as CreateRecipeActivity).replaceFragment(CreateInstructionsFragment())

            }
        }

        return itemView
    }

    private fun verifyIngredientsData(ingredients : String) : Boolean{
        if(ingredients.isBlank()){
            etIngredients?.error = "How could you cook something without any ingredients?"
            return false
        }
        return true
    }

}