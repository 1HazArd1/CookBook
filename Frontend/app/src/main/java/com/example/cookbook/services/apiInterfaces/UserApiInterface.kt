package com.example.cookbook.services.apiInterfaces

import com.example.cookbook.models.User
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface UserApiInterface {

    @POST("Auth/register")
    suspend fun register(@Body user: User): Response<Long>
}