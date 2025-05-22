# ResuMatch

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-69217E?style=for-the-badge&logo=dot-net&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-267BFF?style=for-the-badge&logo=github-actions&logoColor=white)

## 📄 Overview

ResuMatch is an application that helps job seekers match their resume skills against a job description. It's built with a backend that handles all the heavy lifting like AI analysis and a **browser extension** for convenient analysis directly from web pages. You upload your resume, provide a job description, and ResuMatch gives you a detailed breakdown: what skills you have that match, what's missing, a compatibility score, and a quick summary. It's designed to give you clear insights into how well your resume fits a role.

## ✨ Features

* **Resume Text Extraction:** Pulls out all the text from your PDF resumes.

* **Skill Extraction (AI-Powered):** Uses AI to find key skills in both your resume and the job description.

* **Skill Matching:** Compares your skills with the job's requirements to show you what aligns and what you might be missing.

* **Match Scoring:** Gives you a percentage score to quickly see how compatible your resume is with the job.

* **Summary Generation (AI-Powered):** Creates a short, helpful summary of the match results.

* **Modular Processing:** The backend uses a step-by-step system (a "pipeline") to handle all the analysis, making it organized and easy to expand.

* **Temporary File Handling:** Safely stores your uploaded resume files just long enough to process them, then deletes them for privacy.

* **Browser Integration:** (Browser Extension Feature) Analyze job descriptions directly from job board websites without leaving your browser.

## 🏗️ Architecture

ResuMatch works by having two main parts: a **backend API** and a **browser extension**.

The **backend** is where all the analysis happens. It uses a "Pipeline Pattern" – think of it like an assembly line where each step (like extracting text, finding skills, or calculating a score) does one specific job and then passes the information to the next step. This keeps the process organized and makes it easy to add new analysis features.

The **browser extension** acts as a bridge, allowing you to initiate analysis directly from web pages (like job postings). It communicates with the backend API to send job description text and receive analysis results, providing a streamlined workflow.

This setup ensures that the complex analysis is handled efficiently by the backend, while the browser extension provides a smooth and convenient experience for the user.

## 🚀 Getting Started

Follow these steps to get ResuMatch up and running on your local machine.

### Prerequisites

* [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later

* A code editor like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/)

* An API Key for an AI service compatible with OpenRouter (e.g., OpenAI, Anthropic, etc., accessed via [OpenRouter.ai](https://openrouter.ai/)).

### 1. Clone the Repository

```bash
git clone [https://github.com/ThembaFumani/ResuMatch.git](https://github.com/ThembaFumani/ResuMatch.git)
cd ResuMatch
```

### 2. Project Structure

The repository contains the following main projects:

* `ResuMatch.Api`: The ASP.NET Core Web API project (backend).

* `ResuMatch.BrowserExtension`: The web browser extension project.

### 3. API Setup (`ResuMatch.Api`)

1.  **Navigate to the API project directory:**

    ```bash
    cd ResuMatch.Api
    ```

2.  **Configure API Keys and Settings:**
    You will need to configure your API keys and other settings, typically in `appsettings.json` or `appsettings.Development.json`. This includes your OpenRouter API key, the model to use, and the upload directory for temporary files.

3.  **Restore NuGet Packages:**

    ```bash
    dotnet restore
    ```

4.  **Build the Project:**

    ```bash
    dotnet build
    ```

5.  **Run the API:**

    ```bash
    dotnet run
    ```

    The API will typically start on `http://localhost:5250` (or a similar port). You will see the URL in the console output. Make a note of this URL, as your browser extension will need to call it.

### 4. Browser Extension Setup (`ResuMatch.BrowserExtension`)

1.  **Navigate to the browser extension project directory:**

    ```bash
    cd ../ResuMatch.BrowserExtension
    ```

2.  **Configure API Endpoint:**
    The browser extension will need to know where your backend API is running. Look for configuration within its JavaScript files (e.g., `background.js`, `content.js`, or a dedicated config file) and update the API base URL to match your `ResuMatch.Api` address (e.g., `http://localhost:5250`).

3.  **Build the Extension (if required):**
    Some extensions require a build step (e.g., `npm run build`). Check your project's `package.json` for build scripts.

    ```bash
    # npm run build # Example build command if applicable
    ```

4.  **Load the Extension in Your Browser:**

    * **Google Chrome / Microsoft Edge:**

        1.  Open the browser and go to `chrome://extensions` (or `edge://extensions`).

        2.  Enable "Developer mode" (usually a toggle in the top right).

        3.  Click "Load unpacked" and select the `ResuMatch.BrowserExtension` directory (or its `build` folder if a build step was performed).

    * **Mozilla Firefox:**

        1.  Open the browser and go to `about:debugging#/runtime/this-firefox`.

        2.  Click "Load Temporary Add-on..." and select any file inside the `ResuMatch.BrowserExtension` directory (e.g., `manifest.json`).

    The ResuMatch icon should now appear in your browser's toolbar.

## 🧪 Testing

This section provides instructions on how to manually test the backend API and the browser extension.

### 1. Manual Testing

Manual testing allows you to interact with the running application components and verify their functionality.

#### Running Both Applications for End-to-End Testing

To test the full application flow (browser extension interacting with the backend), you need to run both parts concurrently.

1.  **Open Two Separate Terminal Windows:**
    You will need one terminal for the backend API and one for the browser extension (if it has a development server or watch mode).

2.  **Start the Backend API (in Terminal 1):**
    In your first terminal, navigate to the `ResuMatch.Api` directory and start the application:

    ```bash
    cd ResuMatch.Api
    dotnet run
    ```

    Keep this terminal window open and running.

3.  **Start the Browser Extension (in Terminal 2, if applicable):**
    If your browser extension has a development server or watch mode (e.g., `npm start`, `webpack --watch`), start it in a second terminal:

    ```bash
    cd ../ResuMatch.BrowserExtension
    # npm start # Example command if applicable
    ```

    Otherwise, ensure it's loaded as an unpacked extension in your browser as per the setup instructions.

#### Testing the Backend API (via Swagger UI)

You can test the backend API independently using Swagger UI, even while the browser extension is running.

1.  **Access Swagger UI:**
    Open your web browser and go to the Swagger UI endpoint for your API. This is typically `http://localhost:5250/swagger` (replace `5250` with your actual port if different).

2.  **Perform a Resume Analysis:**

    * Locate the relevant endpoint for uploading a resume (e.g., `/api/Resume/UploadResume`).

    * Click on the endpoint to expand it.

    * Click the "Try it out" button.

    * You will see fields to upload a file (your PDF resume) and enter a job description.

    * Upload a sample PDF resume file.

    * Provide a job description in the text area.

    * Click the "Execute" button.

3.  **Verify the API Response:**

    * Observe the "Responses" section in Swagger UI.

    * Check the HTTP status code (e.g., `200 OK` for success).

    * Examine the JSON response body. It should contain the `AnalysisResult` with `MatchScore`, `MatchingSkills`, `MissingSkills`, and `Summary`, and other relevant data.

    * Check for any `Error` messages in the response if the process failed.

#### Testing the Browser Extension

This involves using the extension directly in your browser.

1.  **Navigate to a Job Board Website:**
    Open your browser and go to a website that typically lists job descriptions (e.g., LinkedIn, Indeed, a company's career page).

2.  **Activate the Extension:**
    Click on the ResuMatch browser extension icon in your browser's toolbar. This might open a popup.

3.  **Initiate Analysis:**

    * The extension's UI (in the popup or sidebar) should provide a way to trigger the analysis of the job description currently displayed on the web page. This might involve a button like "Analyze Job" or similar.

    * The extension will likely extract the job description text from the current tab and send it to your `ResuMatch.Api` backend.

4.  **Observe Results:**

    * The extension's UI should then display the `AnalysisResult` (match score, skills, summary) received from the backend.

    * Verify that the analysis is accurate for the job description on the page.

    * Test different job pages to ensure consistent behavior.

By following these steps, you can thoroughly test all components of your ResuMatch application: the automated aspects and the browser-integrated functionality.

## ⚙️ Key Technologies

* **ASP.NET Core 8:** Web API Framework.

* **C#:** Primary programming language.

* **Dependency Injection:** Built-in .NET Core DI for managing services and their dependencies.

* **iText:** For PDF document processing and text extraction.

* **OpenRouter.ai:** For accessing various AI models (e.g., OpenAI, Anthropic) for skill extraction and summary generation.

* **Logging:** Microsoft.Extensions.Logging for application insights.

* **[Add Browser Extension Technologies Here]:** E.g., JavaScript, HTML, CSS (for browser extension development).

## 🤝 Contributing

Contributions are welcome! If you have suggestions for improvements, new features, or bug fixes, please feel free to:

1.  Fork the repository.

2.  Create a new branch (`git checkout -b feature/YourFeature`).

3.  Make your changes.

4.  Commit your changes (`git commit -m 'Add new feature'`).

5.  Push to the branch (`git push origin feature/YourFeature`).

6.  Open a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
