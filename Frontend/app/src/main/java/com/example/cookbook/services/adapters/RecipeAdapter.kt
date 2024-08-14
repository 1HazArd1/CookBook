package com.example.cookbook.services.adapters

import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.cookbook.R
import com.example.cookbook.models.Recipe
import com.squareup.picasso.Callback
import com.squareup.picasso.Picasso
import java.lang.Exception

private const val TAG = "RecipeAdapter"
class RecipeAdapter(
    private val recipes: List<Recipe>?
) : RecyclerView.Adapter<RecipeAdapter.RecipeViewHolder>() {
    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): RecipeViewHolder {

        val itemView =
            LayoutInflater.from(parent.context).inflate(R.layout.recipe_item_layout, parent, false)
        return RecipeViewHolder(itemView)
    }

    override fun onBindViewHolder(holder: RecipeViewHolder, position: Int) {
        val recipe = recipes?.get(position)
        Log.d(TAG,"recipe details: $recipe")
        val recipeDuration = recipe?.duration.toString() + " " +"mins"
        val servings = recipe?.servings?.let {
            if (it.toInt() == 0) "" else "$it people"
        } ?: ""

        if(recipe?.recipeUrl.isNullOrEmpty()){
            holder.recipeImage.setImageResource(R.drawable.default_food_image)
        }else {
            Picasso.get().load(recipe?.recipeUrl).into(holder.recipeImage, object :Callback{
                override fun onSuccess() {
                    Log.d(TAG, "Image loaded successfully")
                }

                override fun onError(e: Exception?) {
                    holder.recipeImage.setImageResource(R.drawable.default_food_image)
                    Log.e(TAG, "Failed to load image", e)
                }

            })
        }
        holder.recipeName.text = recipe?.name
        holder.cuisine.text = recipe?.cuisine
        holder.duration.text = recipeDuration
        holder.servings.text = servings
        holder.seperator.text = if (servings.isNotEmpty()) "." else ""
    }

    override fun getItemCount(): Int {
        return recipes?.size ?: 0
    }

    class RecipeViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        var recipeImage: ImageView = itemView.findViewById(R.id.iv_recipe)
        var recipeName: TextView = itemView.findViewById(R.id.tv_recipe)
        var cuisine: TextView = itemView.findViewById(R.id.tv_recipeCuisine)
        var duration: TextView = itemView.findViewById(R.id.tv_duration)
        var servings: TextView = itemView.findViewById(R.id.tv_servings)
        var seperator : TextView = itemView.findViewById(R.id.tv_seperator)
    }

}