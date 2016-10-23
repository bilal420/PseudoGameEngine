﻿using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using SharpGL;

namespace PseudoGameEngine.graphics
{
    public class Window
    {
        //create a instance of OpenGl
        OpenGL gl = new OpenGL();

        //pointer for Window
        IntPtr win;

        //Window Information
        int Weight, Height;
        string Title;
        bool isFullScreen;
        SDL_WindowFlags Flags;

        //Error Information
        string _error;

        //Creat Context Pointer for OpenGl 
        IntPtr glContext;

        //SDL Event var
        SDL_Event _event;

        bool isWindowOpened;

        public Window(string title,int weight,int height,bool fullscreen = true)
        {
            //Set windows information to private var
            Title = title;
            Weight = weight;
            Height = height;
            isFullScreen = fullscreen;
            // Alow SDL to use OpenGL and Window RESIZABLE
            Flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;
            // Insert flag to make window FullSceen 
            if (fullscreen)
                Flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;

            if (!Init())
                Console.WriteLine(_error);
            else
                isWindowOpened = true;
        }
        bool Init()
        {
            //initializing SDL
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            // return error is unable to initializing SDL
            {
                _error = "Error occurred initializing SDL";
                return false;
            }
            // Create Window with given Information
            win = SDL.SDL_CreateWindow(Title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, Weight, Height, Flags);
            // return error is unable to Create Window
            if (win == null || win == IntPtr.Zero)
            {               
                _error = "Error occurred initializing Window";
                return false;
            }
            initgl();
            return true;
        }

        bool initgl()
        {
            // SDL_GL_CONTEXT_CORE gives us only the newer version, deprecated functions are disabled
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int)SDL_GLattr.SDL_GL_CONTEXT_PROFILE_CORE);
            
            // Turn on double buffering with a 24bit Z buffer.
            // You may need to change this to 16 or 32 for your system 
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DEPTH_SIZE, 16);           

            glContext = SDL_GL_CreateContext(win);

            // return error is unable to Create Window
            if (glContext == null || glContext == IntPtr.Zero)  
            {
                _error = "Error occurred creating OpenGL context";
                return false;
            }

            // Set background color as cornflower blue
            gl.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);
            Clear();
            update();

            return true;
        }
        // Clear color buffer
        public void Clear(){ gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); }

        // Update window with OpenGL rendering
        public void update() { SDL_GL_SwapWindow(win); }

        //Creat delay of Time
        public void Delay(uint Time)  
        {
            SDL_Delay(Time);
        }

        public void Quit()
        {
            isWindowOpened = false;
            // Delete our OpengL context
            SDL_GL_DeleteContext(glContext);
            //Destroy window
            SDL_DestroyWindow(win);
            win = IntPtr.Zero;
            //Quit SDL subsystems
            SDL_Quit();
        }

        #region Event and Updates
        public void SetUpdate(Func<SDL_EventType,int> EventUpdateFunction,Action Update)
        {
            while (isWindowOpened)
            {
                /* Check for new events */
                while (SDL_PollEvent(out _event) == 1)
                {
                    if (_event.type == SDL_EventType.SDL_WINDOWEVENT)
                        if (_event.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                            resetSize();
                    EventUpdateFunction(_event.type);                   
                }
                Update();
            }
        }
        public void SetUpdate(Func<SDL_EventType, int> EventUpdateFunction)
        {
            while (isWindowOpened)
                /* Check for new events */
                while (SDL_PollEvent(out _event) == 1)
                {
                    if (_event.type == SDL_EventType.SDL_WINDOWEVENT)
                        if (_event.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                            resetSize();
                    EventUpdateFunction(_event.type);
                }
        }
        public void SetUpdate(Action Update)
        {
            while (isWindowOpened)
            {                
                Update();
            }
        }

        void resetSize()
        {
            int w, h;
            SDL_GetWindowSize(win,
                        out w,
                        out h);

            Clear();
            gl.Viewport(0, 0, w, h);
            update();
        }

        #endregion
    }
}
