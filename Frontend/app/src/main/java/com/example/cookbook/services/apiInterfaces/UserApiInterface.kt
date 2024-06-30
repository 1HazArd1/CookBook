package com.example.cookbook.services.apiInterfaces

import com.example.cookbook.models.AuthSession
import com.example.cookbook.models.LoginUser
import com.example.cookbook.models.User
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface UserApiInterface {

    @POST("Auth/login")
    suspend fun login(@Body loginUser: LoginUser) : Response<AuthSession>

    @POST("Auth/register")
    suspend fun register(@Body user: User): Response<Long>
}