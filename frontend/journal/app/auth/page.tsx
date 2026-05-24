'use client'
import { FloatingLabelInput } from "@/shared/ui/FloatingLabelInput";
import { Button, Field, Flex, Input } from "@chakra-ui/react";
import { Stack } from "@chakra-ui/react/stack";
import { useForm } from "react-hook-form";

interface FormValues {
    login: string
    password: string
}

const AuthPage = () => {
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<FormValues>();

    const onSubmit = handleSubmit((data) => {
        console.log(data);
    });

    return (
        <Flex justifyContent={"center"}>
            <form onSubmit={onSubmit}>
                <Stack align="flex-start" spaceY={4} >
                    <Field.Root padding={0}>
                        <FloatingLabelInput
                            label="Логин"
                            type="text"
                            {...register("login", {
                                required: {
                                    value: true,
                                    message: "Поле обязательно для заполнения"
                                }
                            }

                            )}
                        />
                        <Field.ErrorText>{errors.login?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root padding={0} marginTop={0}>
                        <FloatingLabelInput
                            label="Пароль"
                            type="password"
                            {...register("password", {
                                required: {
                                    value: true,
                                    message: "Поле обязательно для заполнения"
                                }
                            })} />
                        <Field.ErrorText>{errors.password?.message}</Field.ErrorText>
                    </Field.Root>
                    <Button type="submit">Log in</Button>
                </Stack>
            </form>
        </Flex>
    );
}

export default AuthPage;