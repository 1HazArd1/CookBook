package com.example.cookbook.models

data class AuthSession(
    val userId : Long,
    val email : String,
    val firstName : String,
    val lastName: String?,
    val accessToken : String
)
