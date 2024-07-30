import { useState, ReactDOM } from "react";
import config from "../config.json";

interface UploadBoxProps {
    groupNames: string[];
    fetchGroupNames: () => void;
    setModalIsOpen: (modalIsOpen: boolean) => void;
}

export const UploadBox: React.FC<UploadBoxProps> = ({groupNames, fetchGroupNames, setModalIsOpen}) => { 
    const [files, setFiles] = useState<FileList | null>(null);
    const [status, setStatus] = useState<'initial' | 'uploading' | 'success' | 'fail'>('initial');
    const [group, setGroup] = useState<string | undefined>(undefined);
    const [message, setMessage] = useState<string>("");
    const [showNewGroup, setShowNewGroup] = useState<boolean>(false);
    const [submitClicked, setSubmitClicked] = useState<boolean>(false);
    
    
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setMessage("");
        if (e.target.files) {
            setStatus('initial');
            setFiles(e.target.files);
        }
    }
    const uploadFiles = async(files: FileList) => {
        const uploadPromises = Array.from(files).map(async (file) => {
            const formData = new FormData();
            formData.append("uploadFile", file);
            const uploadURL : string = `http://${config.server_host}:${config.server_port}/Submission/Upload?group=${group}`
            return await fetch(uploadURL, {
                method: 'POST',
                body: formData,
            })
            .catch(error => {
                console.error('Error fetching names:', error);
                setStatus('fail');
            })
        });    
        await Promise.all(uploadPromises);
        setMessage("Files Uploaded and Parsed Successfully");
    }

    const handleUpload = async() => {
        setSubmitClicked(true);
        setMessage("Uploading Files...")
        if (files && group) {
            setStatus('uploading');
            await uploadFiles(files)
            setMessage("Files uploaded and parsed successfully");  
            await fetchGroupNames(); 
        } else if (group) {
            setMessage("Please select files.")
        }   else {
            setMessage("Please select group.")
        }   
    }

    const handleSelectChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        setMessage("");
        const value = event.target.value;
        setGroup(value);
        setShowNewGroup(value === "new");
    }

    return (
        <div className="p-6 h-full">  
            <div className="flex flex-col items-center pb-3">
                <h2 className="font-sans font-bold text-teal text-2xl ">Upload Files</h2>
            </div>
            <div >
                <div className = "input-group pb-2 flex flex-row items-center">
                    <label className = "font-sans font-semibold text-teal text-right text-large w-1/2" htmlFor="dropdown">Select a group:</label>
                    <select className = "font-sans border-b-2 border-teal text teal w-1/2" id="dropdown" value={group ?? undefined} onChange={handleSelectChange}>
                        <option className="font-sans" value = {undefined} >{undefined}</option>
                        {groupNames.map((name, index) => <option className="font-sans text-teal text-xl" value={name} key={index + 1}>{name}</option> )}
                        <option className="font-sans text-teal text-xl" value="new" >New...</option>
                    </select>
                </div>
                {showNewGroup && (
                    <input
                        type="text"
                        className="font-sans text-teal border-b-2 border-teal text-large w-full mt-2"
                        placeholder="New Group ..."
                        value = {group ?? ""}
                        onChange  = {(e) => (setGroup(e.target.value))}
                    />
                )}
            </div>
            <div className = "input-group pb-2">
                <label htmlFor="file" className="sr-only justify-center font-sans">
                    Choose Files
                </label>
                <input id="file" className = "justify-center font-sans font-medium" type="file" multiple onChange={handleFileChange}></input>
            </div>
            
            <div>
                {files && (
                    Array.from(files).map((file, index) => <p key={index} className="font-sans"> {file.name}</p>)
                )}
            </div>
            <div className="flex items-center pt-4 h-1/4">
                <div className="w-1/2 items-center flex justify-center h-full">
                    <button className="bg-teal text-white text-xl font-bold font-sans px-4 py-2 justify-center border-teal border-2 rounded-full hover:bg-white hover:text-teal"onClick={handleUpload} >Submit</button>
                    
                </div>
                <div className="w-1/2 items-center justify-center flex h-full">
                    <button className="bg-teal text-white text-xl font-bold font-sans px-4 py-2 justify-center border-teal border-2 rounded-full hover:bg-white hover:text-teal" onClick={() => setModalIsOpen(false)}> Cancel</button>
                </div>
            </div>
            <div>
                {submitClicked &&  <p className = "justify-center font-sans">{message}</p>}
            </div>
        </div>
    )
}