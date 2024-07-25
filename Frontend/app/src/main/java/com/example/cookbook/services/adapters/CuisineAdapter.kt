package com.example.cookbook.services.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.cookbook.R
import com.example.cookbook.models.Cuisine
import com.squareup.picasso.Picasso

class CuisineAdapter(
    private val cuisines: ArrayList<Cuisine>
) : RecyclerView.Adapter<CuisineAdapter.CuisineViewHolder>() {

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): CuisineViewHolder {

        val itemView =
            LayoutInflater.from(parent.context).inflate(R.layout.cuisine_item_layout, parent, false)

        return CuisineViewHolder(itemView)
    }

    override fun onBindViewHolder(holder: CuisineViewHolder, position: Int) {
        holder.cuisine.text = cuisines[position].cuisine
        Picasso.get().load(cuisines[position].url).into(holder.cuisineImage)
    }

    override fun getItemCount(): Int {
        return cuisines.size
    }

    class CuisineViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val cuisineImage: ImageView = itemView.findViewById(R.id.iv_cuisine)
        val cuisine: TextView = itemView.findViewById(R.id.tv_cuisine)

    }
}