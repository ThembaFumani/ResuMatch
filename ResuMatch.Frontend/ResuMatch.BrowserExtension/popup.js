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
  if (resumeFile) {
    // Enable the analyze button once a file is selected
    analyzeButton.disabled = false;
  } else {
    // Disable , the analyze button if no file is selected
    analyzeButton.disabled = true;
  }
});

analyzeButton.addEventListener("click", () => {
  if (!resumeFile) {
    setError("Please select a resume file.");
    return;
  }
  analyzeButton.style.display = "none";
  loadingButton.style.display = "block";
  resultsDiv.style.display = "none";
  errorDiv.style.display = "none";

  chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
    chrome.tabs.sendMessage(
      tabs[0].id,
      { action: "extractJobDescription" },
      (response) => {
        if (chrome.runtime.lastError) {
          setError("This page is not supported. Please use the extension on a job listing page.");
          resetUI();
          return;
        }
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
  resultsDiv.innerHTML = ''; // Clear previous results

  if (results && typeof results === 'object') {
    if (results.matchScore !== undefined) {
      const scoreElement = document.createElement('p');
      scoreElement.textContent = `Match Score: ${results.matchScore}`;
      resultsDiv.appendChild(scoreElement);
    }

    if (results.summary) { // Check if the summary exists
      const summaryTitle = document.createElement('h3');
      summaryTitle.textContent = 'Summary:';
      resultsDiv.appendChild(summaryTitle);
      const summaryElement = document.createElement('p');
      summaryElement.textContent = results.summary;
      resultsDiv.appendChild(summaryElement);
    }

    if (results.matchingSkills && Array.isArray(results.matchingSkills) && results.matchingSkills.length > 0) {
      const matchingSkillsTitle = document.createElement('h3');
      matchingSkillsTitle.textContent = 'Matching Skills:';
      resultsDiv.appendChild(matchingSkillsTitle);
      const skillsList = document.createElement('ul');
      results.matchingSkills.forEach(skill => {
        const listItem = document.createElement('li');
        listItem.textContent = skill;
        skillsList.appendChild(listItem);
      });
      resultsDiv.appendChild(skillsList);
    }

    if (results.missingSkills && Array.isArray(results.missingSkills) && results.missingSkills.length > 0) {
      const missingSkillsTitle = document.createElement('h3');
      missingSkillsTitle.textContent = 'Missing Skills:';
      resultsDiv.appendChild(missingSkillsTitle);
      const skillsList = document.createElement('ul');
      results.missingSkills.forEach(skill => {
        const listItem = document.createElement('li');
        listItem.textContent = skill;
        skillsList.appendChild(listItem);
      });
      resultsDiv.appendChild(skillsList);
    }
  } else {
    resultsDiv.textContent = "Error: Invalid response format.";
  }

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

// Initialize the analyze button as disabled
analyzeButton.disabled = true;