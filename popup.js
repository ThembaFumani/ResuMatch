// popup.js
const fileInput = document.getElementById("fileInput");
const analyzeButton = document.getElementById("analyzeButton");
const loadingButton = document.getElementById("loadingButton");
const resultsDiv = document.getElementById("results");
const errorDiv = document.getElementById("error");

let resumeFile = null;

fileInput.addEventListener("change", (event) => {
  window.focus();
  console.log('File selected, attempting to refocus popup.');
  resumeFile = event.target.files[0]; // Store the selected file

  chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
    chrome.tabs.sendMessage(
      tabs[0].id,
      { action: "getJobDescription" },
      (response) => {
        if (response && response.jobDescription) {
          const jobDescription = response.jobDescription;
          sendDataToBackend(resumeFile, jobDescription);
        } else {
          setError(response ? response.error : "Could not get job description");
          resetUI();
        }
      }
    );
  });
});

analyzeButton.addEventListener("click", () => {
  if (!fileInput.files[0]) {
    setError("Please select a resume file.");
    return;
  }
  analyzeButton.style.display = "none";
  loadingButton.style.display = "block";
  resultsDiv.style.display = "none";
  errorDiv.style.display = "none";
});

function sendDataToBackend(file, jobDescription) {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("jobDescription", jobDescription);

  const apiUrl = "http://localhost:5250/api/resume/upload";

  fetch(apiUrl, {
    method: "POST",
    body: formData,
  })
    .then((response) => {
      if (!response.ok) {
        return response.text().then((text) => {
          throw new Error(
            `HTTP error! Status: ${response.status}, Response: ${text}`
          );
        }
        );
      }
      return response.json();
    })
    .then((data) => {
      displayResults(data);
      resetUI();
    })
    .catch((error) => {
      setError(error.message);
      resetUI();
    });
}

function displayResults(results) {
  resultsDiv.textContent = JSON.stringify(results, null, 2);
  resultsDiv.style.display = "block";
}

function setError(message) {
  errorDiv.textContent = message;
  errorDiv.style.display = "block";
}

function resetUI() {
  analyzeButton.style.display = "block";
  loadingButton.style.display = "none";
}