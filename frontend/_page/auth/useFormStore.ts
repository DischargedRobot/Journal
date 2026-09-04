import { create } from "zustand"
import { FormValues, LOGIN_FIELDS, PERSONAL_FIELDS, REQUIRED_FIELDS } from "./fields"

type TFormField = {
    name: keyof FormValues;
    data?: string;
    error?: string;
    required: boolean;
}
  
interface IFormStore {
    formData: TFormField[]
    updateField: (name: keyof FormValues, data: string ) => void
    
}


export const useFormStore = create<IFormStore>((set) => {
    const isRequired = (name: keyof FormValues): boolean => {
        return (
          REQUIRED_FIELDS.includes(name)
        )
    }

    const initialFields: TFormField[] = [
        ...PERSONAL_FIELDS.map((name) => ({
          name,
          data: '', 
          error: '',
          required: isRequired(name),
        })),
        ...LOGIN_FIELDS.map((name) => ({
          name,
          data: '',
          error: '',
          required: isRequired(name),
        })),
    ]

    return {
        formData: initialFields,
        updateField: (fieldName, fieldData) => {
            set((state) => {
                const existingIndex = state.formData.findIndex(
                    (field) => field.name === fieldName
                );
        
                if (existingIndex !== -1) {
                    const updatedFields = [...state.formData];
                    updatedFields[existingIndex] = {
                        ...updatedFields[existingIndex],
                        data: fieldData,
                    };
                    return { formData: updatedFields };
                }
        
                return state
            })
        },
    }
})