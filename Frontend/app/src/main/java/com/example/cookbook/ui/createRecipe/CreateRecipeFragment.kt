package com.example.cookbook.ui.createRecipe

import android.content.Context
import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.Toast
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.fragment.app.Fragment
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import com.example.cookbook.R
import com.example.cookbook.models.Recipe
import com.example.cookbook.services.apiInterfaces.RecipeApiInterface
import com.example.cookbook.services.apiService.RetrofitService
import com.example.cookbook.services.sharedPreference.PreferenceManager
import com.google.android.material.textfield.TextInputEditText
import com.google.firebase.firestore.FirebaseFirestore
import com.google.firebase.storage.FirebaseStorage
import kotlinx.coroutines.CompletableDeferred
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import retrofit2.HttpException
import java.io.IOException
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException
import kotlin.coroutines.suspendCoroutine

private const val TAG = "CreateRecipeFragment"

class CreateRecipeFragment : Fragment() {

    private var recipeImageUri: Uri? = null
    private lateinit var pickMedia: ActivityResultLauncher<PickVisualMediaRequest>
    private var imageSelectionCallback: ((Uri?) -> Unit)? = null

    private lateinit var communicator: RecipeCommunicator

    override fun onAttach(context: Context) {
        super.onAttach(context)
        if(context is RecipeCommunicator){
            communicator = context
        }
        else{
            throw RuntimeException("$context must implement RecipeCommunicator")
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        pickMedia = registerForActivityResult(ActivityResultContracts.PickVisualMedia()) { uri ->
            if (uri != null) {
                Log.d(TAG, "Selected URI: $uri")
                recipeImageUri = uri
                Log.d(TAG, "$recipeImageUri")
            } else {
                Log.d(TAG, "No media selected")
            }
            imageSelectionCallback?.invoke(uri)
        }
    }

    private var itemView: View? = null

    private var etRecipeName: TextInputEditText? = null
    private var etCuisineName: TextInputEditText? = null
    private var etDurationHours: TextInputEditText? = null
    private var etDurationMins: TextInputEditText? = null
    private var etServings: TextInputEditText? = null
    private var btnNext: ImageButton? = null
    private var btnRecipeImage: ImageButton? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {

        itemView = inflater.inflate(R.layout.create_recipe_fragment, container, false)

        etRecipeName = itemView?.findViewById(R.id.et_recipeName)
        etCuisineName = itemView?.findViewById(R.id.et_cuisine)
        etDurationHours = itemView?.findViewById(R.id.et_duration_hours)
        etDurationMins = itemView?.findViewById(R.id.et_duration_mins)
        etServings = itemView?.findViewById(R.id.et_servings)
        btnNext = itemView?.findViewById(R.id.btn_recipe_next)
        btnRecipeImage = itemView?.findViewById(R.id.btn_Recipe_Image)

        btnRecipeImage?.setOnClickListener {

            lifecycleScope.launch {
                val selectedImageUri = selectImageFromDevice()
                if (selectedImageUri != null) {
                    recipeImageUri = selectedImageUri
                    btnRecipeImage?.setBackgroundResource(R.drawable.round_button_gray)
                    btnRecipeImage?.setImageResource(R.drawable.photo_image_gray)
                    btnRecipeImage?.apply {
                        isEnabled = false
                        isClickable = false
                    }
                    Toast.makeText(context, "Image selected", Toast.LENGTH_SHORT).show()
                    Log.d(TAG, "URI of the selected image: $recipeImageUri")
                } else {
                    Log.e(TAG, "No image selected")
                }
            }
        }
        btnNext?.setOnClickListener {

            if (verifyRecipeData()) {

                val recipeName = etRecipeName?.text.toString().trim()
                lifecycleScope.launch {
                    try {
                        val uploadTask = CompletableDeferred<String?>()
                        recipeImageUri?.let { uri ->
                            context?.let { ctx ->
                                uploadRecipeImage(uri, recipeName, ctx) { success, imageUrl ->
                                    if (success) {
                                        uploadTask.complete(imageUrl)
                                    } else {
                                        uploadTask.complete(null)
                                    }
                                }
                            }
                        } ?: uploadTask.complete(null)
                        // get the url of the uploaded image
                        val uploadedImageUrl = uploadTask.await()

                        Log.d(TAG, "Uploaded image url: $uploadedImageUrl")

                        val durationInHours = try {
                            etDurationHours?.text.toString().toInt()
                        } catch (e: NumberFormatException) {
                            0
                        }
                        val durationInMins = try {
                            etDurationMins?.text.toString().toInt()
                        } catch (e: NumberFormatException) {
                            0
                        }
                        val recipe = Recipe(
                            id = null,
                            name = formatString(recipeName),
                            cuisine = etCuisineName?.text.toString().trim(),
                            duration = durationInHours * 60 + durationInMins,
                            servings = try {
                                etServings?.text.toString().toByte()
                            } catch (e: Exception) {
                                null
                            },
                            recipeUrl = uploadedImageUrl,
                            isEditable = null
                        )
                        // pass the created recipe to the instruction fragment to submit
                        communicator.passRecipeData(recipe)
                        //replace this fragment
                        (activity as CreateRecipeActivity).replaceFragment(CreateIngredientsFragment())

                        Log.d(TAG, "$recipe")
//                        val recipeId = createRecipe(recipe)
//                        Log.d(TAG,"$recipeId")

                    } catch (e: Exception) {
                        Log.e(TAG, "Something went wrong: ${e.message}")
                    }
                }
            }

        }

        return itemView
    }

    private fun verifyRecipeData(): Boolean {

        if (etRecipeName?.text.toString().isBlank()) {
            etRecipeName?.error = "Recipe Name is Mandatory"
            return false
        }
        if (etDurationMins?.text.toString().isBlank()) {
            etDurationMins?.error = "Duration of preparation is mandatory"
            return false
        }

        return true
    }

    private fun getRetrofitClient(): RecipeApiInterface {
        val retrofitService = RetrofitService(requireContext())
        return retrofitService.retrofit.create(RecipeApiInterface::class.java)
    }

//    private suspend fun createRecipe(recipe: Recipe): Long = suspendCoroutine { continuation ->
//        lifecycleScope.launch {
//            lifecycle.repeatOnLifecycle(Lifecycle.State.RESUMED) {
//                val retrofitClient = getRetrofitClient()
//                Log.d(TAG, "Calling recipe API")
//                val response = try {
//                    retrofitClient.createUserRecipe(recipe)
//                } catch (e: IOException) {
//                    Log.e(TAG, "I/O Exception ${e.message}")
//                    return@repeatOnLifecycle
//                } catch (e: HttpException) {
//                    Log.e(TAG, "Http Exception ${e.message}")
//                    return@repeatOnLifecycle
//                } catch (e: Exception) {
//                    Log.e(TAG, "Exception ${e.message}")
//                    return@repeatOnLifecycle
//                }
//                when {
//                    response.isSuccessful && response.body() != null -> {
//                        Log.d(TAG, "Received response")
//                        withContext(Dispatchers.Main) {
//                            val recipeId = response.body()!!
//                            continuation.resume(recipeId)
//                        }
//                    }
//
//                    response.isSuccessful && response.body() == null -> {
//                        // successfully fetched api but received null response
//                        Log.d(TAG, "Response received but with no body")
//                        continuation.resumeWithException(Exception("Response received with no body"))
//                    }
//
//                    else -> {
//                        // any other case like unauthorised response
//                        val errorResponse = response.toString()
//                        Log.e(TAG, "error : $errorResponse")
//                        continuation.resumeWithException(Exception("API error: $errorResponse"))
//                    }
//                }
//
//            }
//        }
//    }

    private suspend fun selectImageFromDevice(): Uri? = suspendCoroutine { continuation ->
        imageSelectionCallback = { uri ->
            continuation.resume(uri)
        }
        pickMedia.launch(PickVisualMediaRequest(ActivityResultContracts.PickVisualMedia.ImageOnly))
    }

    private fun uploadRecipeImage(
        imageUri: Uri,
        recipeName: String,
        context: Context,
        callback: (Boolean, String?) -> Unit
    ) {
        val fileName = generateFileName(recipeName, context)
        Log.d(TAG, "File Name: $fileName")
        val refStorage = FirebaseStorage.getInstance().reference.child("recipe_images/$fileName")

        refStorage.putFile(imageUri)
            .addOnSuccessListener { taskSnapshot ->
                taskSnapshot.storage.downloadUrl.addOnSuccessListener { uri ->
                    val imageUrl = uri.toString()
                    // Save the image URL to Firebase
                    saveImage(imageUrl, refStorage.path)
                    callback(true, imageUrl)
                }
            }
            .addOnFailureListener { e ->
                // Handle any errors here
                Toast.makeText(context, "Failed to upload", Toast.LENGTH_SHORT).show()
                Log.e(TAG, "Error uploading image: ${e.message}")
                callback(false, null)
            }
    }

    private fun saveImage(imageUrl: String, imagePath: String) {
        val firestoreRef = FirebaseFirestore.getInstance().collection("recipe_images")

        val imageData = hashMapOf(
            "url" to imageUrl,
            "path" to imagePath
        )

        firestoreRef.add(imageData)
            .addOnSuccessListener {
                // Image URL and metadata saved successfully
                Log.d(TAG, "Image stored successfully")
            }
            .addOnFailureListener { e ->
                // Handle any errors in saving the image data
                Log.e(TAG, "Something went wrong: ${e.message}")
            }

    }

    private fun generateFileName(recipeName: String, context: Context): String {
        val preferenceManager = PreferenceManager(context)
        val userId = preferenceManager.getUserId()?.toLong()
        if (userId == 23L)
            return "recipe_admin_${recipeName}_${System.currentTimeMillis()}.jpg"
        return "recipe_user_${recipeName}_${System.currentTimeMillis()}.jpg"
    }

    private fun formatString(input: String): String {
        return input.lowercase()
            .split(" ") // Split by spaces
            .joinToString(" ") { it.replaceFirstChar { char -> char.uppercase() } }
    }
}