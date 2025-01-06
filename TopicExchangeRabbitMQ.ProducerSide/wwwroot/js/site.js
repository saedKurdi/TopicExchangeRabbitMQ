async function addGroupToRedisAsync(event) {
    event.preventDefault();

    const groupName = document.getElementById("groupName").value;

    // Ensure groupName is not empty
    if (!groupName || groupName.trim() === "") {
        alert("Group name cannot be empty!");
        return;
    }

    // Axios POST request with JSON payload
    await axios.post('/Producer/AddGroup', { groupName: groupName })
        .then(response => {
            console.log('Group added successfully:', response.data);
            alert(`Group '${response.data.name}' added successfully!`);
            location.href = "/Producer/Index";
        })
        .catch(error => {
            console.error('Error adding group:', error);
            alert('Failed to add group. Please try again.');
        });
}

async function sendMessageToGroupsAsync(event) {
    event.preventDefault();

    const messageText = document.getElementById("messageText").value;
    if (!messageText || messageText.trim() === "") {
        alert("Message text cannot be empty!");
        return;
    }

    const checkboxes = document.querySelectorAll("input[type='checkbox']:checked");
    const selectedGroups = Array.from(checkboxes).map(cb => cb.parentElement.textContent.trim());
    console.log(selectedGroups);

    if (selectedGroups.length === 0) {
        alert("No groups selected!");
        return;
    }

    await axios.post('/Producer/SendMessageToGroups', {
        message: messageText,
        groups: selectedGroups
    }).then(response => {
        alert(response.data);
    }).catch(error => {
        console.error('Error sending message:', error);
        alert('Failed to send message. Please try again.');
    });
}

function loadSubgroupsOfSelectedGroup(groupName) {
    const group = document.getElementById(groupName);

    // Check if subgroups are already loaded
    if (group.getAttribute('data-loaded') === 'true') {
        return;
    }

    axios.get(`/Producer/GetSubGroupsOfGroup?groupName=${groupName}`)
        .then(response => {
            const subgroups = response.data;

            // Avoid duplicates and handle nesting
            subgroups.forEach(subgroup => {
                if (!document.getElementById(subgroup)) {
                    const subgroupElement = document.createElement("div");
                    subgroupElement.id = subgroup;
                    subgroupElement.style = "display:flex;flex-direction:column;cursor:pointer; margin-left:20px;";
                    subgroupElement.onclick = () => loadSubgroupsOfSelectedGroup(subgroup);

                    const label = document.createElement("label");
                    label.innerHTML = `<input type="checkbox"/> ${subgroup}`;
                    subgroupElement.appendChild(label);

                    group.appendChild(subgroupElement);
                }
            });

            // Mark this group as loaded
            group.setAttribute('data-loaded', 'true');
        })
        .catch(error => {
            console.error('Error getting subgroups:', error);
            alert("Failed to load subgroups. Please try again.");
        });
}
